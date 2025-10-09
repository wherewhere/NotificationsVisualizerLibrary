using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Model.BaseElements;
using NotificationsVisualizerLibrary.Parsers;
using NotificationsVisualizerLibrary.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary
{
    public sealed partial class PreviewXboxToast : UserControl, IPreviewToast
    {
        private static readonly XmlTemplateParser _parser = new XmlTemplateParser();

        public PreviewXboxToast()
        {
            InitializeComponent();

            CurrFeatureSet = FeatureSet.Get(DeviceFamily, OSBuildNumber);
        }

        private DataBindingValues _lastDataBindingValues;
        public ParseResult Initialize(XmlDocument content, PreviewNotificationData data)
        {
            ParseResult result = _parser.ParseToast(content.GetXml(), CurrFeatureSet);

            if (result.IsOkForRender())
            {
                _lastDataBindingValues = new DataBindingValues(data);
                result.Toast.ApplyDataBinding(_lastDataBindingValues);

                if (result.IsOkForRender())
                {
                    InitializeContent(result.Toast);
                }
            }

            return result;
        }

        //private readonly string _currLaunch = "";
        //private readonly ActivationType _currActivationType = ActivationType.Foreground;
        //private readonly Dictionary<string, FrameworkElement> _elementsWithIds;

        //private IToast _currContent;

        public bool HasContent { get; private set; }

        private void ReInitializeContent()
        {
            IToast _currContent = null;
            InitializeContent(_currContent);
        }

        private void InitializeContent(IToast toastContent)
        {
            HasContent = false;

            TextBlockTitle.Text = "";
            TextBlockSubtitle.Text = "";
            TextBlockSubtitle.Visibility = Visibility.Collapsed;

            ImageAppLogoOverride.Visibility = Visibility.Collapsed;
            CircleImageAppLogoOverride.Visibility = Visibility.Collapsed;

            if (toastContent != null)
            {
                if (toastContent.Visual != null)
                {
                    Visual visual = toastContent.Visual;

                    AdaptiveBinding binding = visual.Bindings.FirstOrDefault();

                    if (binding != null)
                    {
                        HasContent = true;

                        AdaptiveContainer container = binding.Container;

                        List<AdaptiveTextField> texts = container.Children.OfType<AdaptiveTextField>().ToList();

                        AdaptiveTextField titleText = texts.ElementAtOrDefault(0);
                        if (titleText != null)
                        {
                            TextBlockTitle.Text = titleText.Text;
                        }

                        AdaptiveTextField bodyTextLine1 = texts.ElementAtOrDefault(1);
                        if (bodyTextLine1 != null)
                        {
                            TextBlockSubtitle.Text = bodyTextLine1.Text;
                            TextBlockSubtitle.Visibility = Visibility.Visible;
                        }

                        AdaptiveImage appLogoOverride = container.Children.OfType<AdaptiveImage>().FirstOrDefault(i => i.Placement == Model.Enums.Placement.AppLogoOverride);
                        if (appLogoOverride != null)
                        {
                            BitmapImage bmp = ImageHelper.GetBitmap(appLogoOverride.Src);
                            if (appLogoOverride.HintCrop == Model.Enums.HintCrop.Circle)
                            {
                                CircleImageAppLogoOverride.Source = bmp;
                                CircleImageAppLogoOverride.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ImageAppLogoOverride.Source = bmp;
                                ImageAppLogoOverride.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }
        }

        private static readonly DependencyProperty PropertiesProperty =
            DependencyProperty.Register(
                nameof(Properties),
                typeof(PreviewToastProperties),
                typeof(PreviewXboxToast),
                new PropertyMetadata(new PreviewToastProperties()));

        public PreviewToastProperties Properties
        {
            get => GetValue(PropertiesProperty) as PreviewToastProperties;
            set => SetValue(PropertiesProperty, value);
        }

        #region DeviceFamily

        private static readonly DependencyProperty DeviceFamilyProperty =
            DependencyProperty.Register(
                nameof(DeviceFamily),
                typeof(DeviceFamily),
                typeof(PreviewXboxToast),
                new PropertyMetadata(FeatureSet.GetCurrentDeviceFamily(), OnDeviceFamilyChanged));

        /// <summary>
        /// Gets or sets the current device family, which impacts what features are available on the tiles.
        /// </summary>
        public DeviceFamily DeviceFamily
        {
            get => (DeviceFamily)GetValue(DeviceFamilyProperty);
            set => SetValue(DeviceFamilyProperty, value);
        }

        private static void OnDeviceFamilyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewXboxToast).OnDeviceFamilyChanged(e);
        }

        private void OnDeviceFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            // Feature set is affected
            UpdateFeatureSet();
        }

        #endregion

        private static readonly DependencyProperty OSBuildNumberProperty =
            DependencyProperty.Register(
                nameof(OSBuildNumber),
                typeof(int),
                typeof(PreviewXboxToast),
                new PropertyMetadata(FeatureSet.GetCurrentOSBuildNumber(), OnOSBuildNumberChanged));

        /// <summary>
        /// Gets or sets the current OS version, which impacts what features and bug fixes are available.
        /// </summary>
        public int OSBuildNumber
        {
            get => (int)GetValue(OSBuildNumberProperty);
            set => SetValue(OSBuildNumberProperty, value);
        }

        private static void OnOSBuildNumberChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as PreviewXboxToast).OnOSBuildNumberChanged(e);
        }

        private void OnOSBuildNumberChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateFeatureSet();
        }

        internal FeatureSet CurrFeatureSet { get; private set; }

        private void UpdateFeatureSet()
        {
            CurrFeatureSet = FeatureSet.Get(DeviceFamily, OSBuildNumber);

            ReInitializeContent();
        }

        public ParseResult Initialize(XmlDocument content)
        {
            return Initialize(content, null);
        }

        public void Update(PreviewNotificationData data)
        {
            // No-op for now
        }
    }
}
