using System.Collections.Generic;
using System.Linq;

namespace NotificationsVisualizerLibrary.Model
{
    internal abstract class AdaptiveParentElement(NotificationType context, FeatureSet supportedFeatures) : AdaptiveChildElement(context, supportedFeatures)
    {
        /// <summary>
        /// Returns true if this element or any descendants use data binding
        /// </summary>
        /// <returns></returns>
        public bool AnyUseDataBinding() => UsesDataBinding || GetAllDescendants().Any(i => i.UsesDataBinding);

        internal abstract IEnumerable<AdaptiveChildElement> GetAllChildren();

        internal IEnumerable<AdaptiveChildElement> GetAllDescendants()
        {
            foreach (AdaptiveChildElement child in GetAllChildren())
            {
                yield return child;

                if (child is AdaptiveParentElement element)
                {
                    foreach (AdaptiveChildElement descendant in element.GetAllDescendants())
                    {
                        yield return descendant;
                    }
                }
            }
        }

        internal override void ApplyDataBinding(DataBindingValues dataBinding)
        {
            ApplyDataBindingToThisInstance(dataBinding);

            foreach (AdaptiveChildElement child in GetAllChildren())
            {
                child.ApplyDataBinding(dataBinding);
            }
        }
    }
}
