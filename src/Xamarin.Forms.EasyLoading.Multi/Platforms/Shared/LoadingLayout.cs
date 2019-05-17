using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.EasyLoading
{
    public static class LoadingLayout
    {
        public static readonly BindableProperty IsLoadingProperty = BindableProperty.CreateAttached("IsLoading", typeof(bool), typeof(Layout<View>), default(bool), defaultBindingMode: BindingMode.TwoWay, propertyChanged: (b, o, n) => OnIsLoadingChanged(b, (bool)o, (bool)n));
        public static readonly BindableProperty LoadingTemplateProperty = BindableProperty.CreateAttached("LoadingTemplate", typeof(DataTemplate), typeof(Layout<View>), default(DataTemplate), propertyChanged: (b, o, n) => { GetLoadingLayoutController(b).LoadingTemplate = (DataTemplate)n; });

        // TODO: Figure out if a selector makes sense...
        public static readonly BindableProperty LoadingTemplateSelectorProperty =
        BindableProperty.CreateAttached("LoadingTemplateSelector", typeof(DataTemplateSelector), typeof(Layout<View>), default(DataTemplateSelector),
        propertyChanged: (b, o, n) => { GetLoadingLayoutController(b).LoadingTemplateSelector = (DataTemplateSelector)n; });

        static readonly BindableProperty LoadingLayoutControllerProperty =
             BindableProperty.CreateAttached("LoadingLayoutController", typeof(LoadingLayoutController), typeof(Layout<View>), default(LoadingLayoutController),
                 defaultValueCreator: (b) => new LoadingLayoutController((Layout<View>)b),
                 propertyChanged: (b, o, n) => OnControllerChanged(b, (LoadingLayoutController)o, (LoadingLayoutController)n));

        public static void SetIsLoading(BindableObject b, bool value)
        {
            b.SetValue(IsLoadingProperty, value);
        }

        public static bool GetIsLoading(BindableObject b)
        {
            return (bool)b.GetValue(IsLoadingProperty);
        }

        public static void SetLoadingTemplate(BindableObject b, DataTemplate value)
        {
            b.SetValue(LoadingTemplateProperty, value);
        }

        public static DataTemplate GetLoadingTemplate(BindableObject b)
        {
            return (DataTemplate)b.GetValue(LoadingTemplateProperty);
        }

        // TODO: Figure out if a selector makes sense...
         public static void SetLoadingTemplateSelector(BindableObject b, DataTemplateSelector value)
         {
             b.SetValue(LoadingTemplateSelectorProperty, value);
         }

         public static DataTemplateSelector GetLoadingTemplateSelector(BindableObject b)
         {
             return (DataTemplateSelector)b.GetValue(LoadingTemplateSelectorProperty);
         }

        static void OnIsLoadingChanged(BindableObject bindable, bool oldValue, bool newValue)
        {
            // Swap out the current children for the Loading Template.
            if (oldValue != newValue && newValue)
            {
                GetLoadingLayoutController(bindable).SwitchToLoadingTemplate();
            }
            else if (oldValue != newValue && !newValue)
            {
                GetLoadingLayoutController(bindable).SwitchToContent();
            }
        }

        static LoadingLayoutController GetLoadingLayoutController(BindableObject b)
        {
            return (LoadingLayoutController)b.GetValue(LoadingLayoutControllerProperty);
        }

        static void SetLoadingLayoutController(BindableObject b, LoadingLayoutController value)
        {
            b.SetValue(LoadingLayoutControllerProperty, value);
        }

        static void OnControllerChanged(BindableObject b, LoadingLayoutController oldC, LoadingLayoutController newC)
        {
            if (newC == null)
            {
                return;
            }

            newC.LoadingTemplate = GetLoadingTemplate(b);

            // TODO: Figure out if a selector makes sense...
             newC.LoadingTemplateSelector = GetLoadingTemplateSelector(b);
        }
    }

    class LoadingLayoutController
    {
        readonly WeakReference<Layout<View>> _layoutWeakReference;
        DataTemplate _loadingTemplate;

        // TODO: Figure out if a selector makes sense...
        DataTemplateSelector _loadingTemplateSelector;

        private IList<View> _originalContent;

        public DataTemplate LoadingTemplate { get => _loadingTemplate; set => SetLoadingTemplate(value); }

        // TODO: Figure out if a selector makes sense...
        public DataTemplateSelector LoadingTemplateSelector { get => _loadingTemplateSelector; set => SetLoadingTemplateSelector(value); }

        public LoadingLayoutController(Layout<View> layout)
        {
            _layoutWeakReference = new WeakReference<Layout<View>>(layout);
        }

        void SetLoadingTemplate(DataTemplate loadingTemplate)
        {
            if (loadingTemplate is DataTemplateSelector)
            {
                throw new NotSupportedException($"You are using an instance of {nameof(DataTemplateSelector)} to set the {nameof(LoadingLayout)}.{LoadingLayout.LoadingTemplateProperty.PropertyName} property. Please provide a DataTemplate instead.");
            }

            _loadingTemplate = loadingTemplate;
        }

        // TODO: Figure out if a selector makes sense...
        void SetLoadingTemplateSelector(DataTemplateSelector loadingTemplateSelector)
        {
            _loadingTemplateSelector = loadingTemplateSelector;
        }

        public void SwitchToContent()
        {
            Layout<View> layout;

            if (!_layoutWeakReference.TryGetTarget(out layout))
            {
                return;
            }

            // Put the original content back in.
            layout.Children.Clear();

            foreach (var item in _originalContent)
            {
                layout.Children.Add(item);
            }
        }

        public void SwitchToLoadingTemplate()
        {
            Layout<View> layout;

            if (!_layoutWeakReference.TryGetTarget(out layout))
            {
                return;
            }

            // Put the original content somewhere where we can restore it.
            _originalContent = new List<View>();

            foreach (var item in layout.Children)
                _originalContent.Add(item);

            // Add the loading template.
            layout.Children.Clear();
            layout.Children.Add(CreateItemView(layout));
        }

        /// <summary>
        /// Expand the LoadingDataTemplate or use the template selector.
        /// </summary>
        /// <returns>The item view.</returns>
        View CreateItemView(Layout<View> layout)
        {
            return CreateItemView(_loadingTemplate ?? _loadingTemplateSelector?.SelectTemplate(null, layout));
        }

        /// <summary>
        /// Expand the Loading Data Template.
        /// </summary>
        /// <returns>The item view.</returns>
        /// <param name="dataTemplate">Data template.</param>
        View CreateItemView(DataTemplate dataTemplate)
        {
            if (dataTemplate != null)
            {
                var view = (View)dataTemplate.CreateContent();
                return view;
            }
            else
            {
                return new Label() { Text = "Loading..." };
            }
        }
    }
}
