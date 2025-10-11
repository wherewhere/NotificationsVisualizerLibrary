using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Parsers;
using System;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NotificationsVisualizerLibrary.Renderers
{
    /// <summary>
    /// A control that renders adaptive content.
    /// </summary>
    [TemplatePart(Name = "PART_AdaptiveContainer", Type = typeof(Border))]
    public sealed partial class PreviewAdaptiveContent : Control
    {
        private Border? _adaptiveContainer;

        public double ExternalMargin { get; set; } = 8;

        /// <summary>
        /// Constructs a new PreviewAdaptiveContent control.
        /// </summary>
        public PreviewAdaptiveContent() => DefaultStyleKey = typeof(PreviewAdaptiveContent);

        /// <summary>
        /// Obtain the AdaptiveStackPanel root
        /// </summary>
        [MemberNotNull(nameof(_adaptiveContainer))]
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _adaptiveContainer = GetTemplateChild("PART_AdaptiveContainer") as Border;

            if (_adaptiveContainer == null)
            {
                throw new NullReferenceException("Template parts not available");
            }
        }

        /// <summary>
        /// Display the adaptive content
        /// </summary>
        /// <param name="adaptiveContent"></param>
        public ParseResult Initialize(string adaptiveContent)
        {
            ParseResult result = XmlTemplateParser.ParseAdaptiveContent(adaptiveContent, FeatureSet.Get(FeatureSet.GetCurrentDeviceFamily(), FeatureSet.GetCurrentOSBuildNumber()));

            if (result.IsOkForRender())
            {
                InitializeContent(result.AdaptiveContent!);
            }

            return result;
        }

        private void InitializeContent(AdaptiveContainer adaptiveContent)
        {
            Reset();

            _adaptiveContainer!.Child = AdaptiveRenderer.Render(adaptiveContent, new Thickness(GetExternalMargin()));
        }

        private void Reset() => _adaptiveContainer!.Child = null;

        private double GetExternalMargin() => ExternalMargin;
    }
}
