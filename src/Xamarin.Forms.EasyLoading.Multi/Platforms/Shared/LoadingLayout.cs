using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.EasyLoading;

namespace Xamarin.Forms.EasyLoading
{
    public static class LoadingLayout
    {
        public static readonly BindableProperty IsLoadingProperty = BindableProperty.CreateAttached("IsLoading",
            typeof(bool), typeof(Layout<View>), default(bool),
            propertyChanged: (b, o, n) => OnIsLoadingChanged(b, (bool) o, (bool) n));

        public static readonly BindableProperty LoadingTemplateProperty = BindableProperty.CreateAttached(
            "LoadingTemplate", typeof(DataTemplate), typeof(Layout<View>), default(DataTemplate),
            propertyChanged: (b, o, n) => { GetLoadingLayoutController(b).LoadingTemplate = (DataTemplate) n; });

        public static readonly BindableProperty IsEmptyProperty = BindableProperty.CreateAttached("IsEmpty",
            typeof(bool), typeof(Layout<View>), default(bool),
            propertyChanged: (b, o, n) => OnIsEmptyChanged(b, (bool) o, (bool) n));



        public static readonly BindableProperty EmptyTemplateProperty = BindableProperty.CreateAttached("EmptyTemplate",
            typeof(DataTemplate), typeof(Layout<View>), default(DataTemplate),
            propertyChanged: (b, o, n) => { GetLoadingLayoutController(b).LoadingTemplate = (DataTemplate) n; });




        public static readonly BindableProperty RepeatCountProperty =
            BindableProperty.CreateAttached("RepeatCount", typeof(int), typeof(Layout<View>), defaultValue: 1,
                propertyChanged: (b, o, n) => { GetLoadingLayoutController(b).RepeatCount = (int) n; });

        // TODO: Figure out if a selector makes sense...
        public static readonly BindableProperty LoadingTemplateSelectorProperty =
            BindableProperty.CreateAttached("LoadingTemplateSelector", typeof(DataTemplateSelector),
                typeof(Layout<View>), default(DataTemplateSelector),
                propertyChanged: (b, o, n) =>
                {
                    GetLoadingLayoutController(b).LoadingTemplateSelector = (DataTemplateSelector) n;
                });

        static readonly BindableProperty LoadingLayoutControllerProperty =
            BindableProperty.CreateAttached("LoadingLayoutController", typeof(LoadingLayoutController),
                typeof(Layout<View>), default(LoadingLayoutController),
                defaultValueCreator: (b) => new LoadingLayoutController((Layout<View>) b, 1),
                propertyChanged: (b, o, n) =>
                    OnControllerChanged(b, (LoadingLayoutController) o, (LoadingLayoutController) n));

        static readonly BindableProperty EmptyLayoutControllerProperty =
            BindableProperty.CreateAttached("EmptyLayoutController", typeof(EmptyLayoutController),
                typeof(Layout<View>), default(EmptyLayoutController),
                defaultValueCreator: (b) => new EmptyLayoutController((Layout<View>)b, 1), 
                propertyChanged: (b, o, n) =>
                    OnEmptyControllerChanged(b, (EmptyLayoutController)o, (EmptyLayoutController)n));


        public static void SetIsEmpty(BindableObject b, bool value)
        {
            b.SetValue(IsEmptyProperty, value);
        }

        public static bool GetIsEmpty(BindableObject b)
        {
            return (bool) b.GetValue(IsEmptyProperty);
        }

        public static void SetEmptyTemplate(BindableObject b, DataTemplate value)
        {
            b.SetValue(EmptyTemplateProperty, value);
        }

        public static DataTemplate GetEmptyTemplate(BindableObject b)
        {
            return (DataTemplate) b.GetValue(EmptyTemplateProperty);
        }

        public static void SetRepeatCount(BindableObject b, int value)
        {
            b.SetValue(RepeatCountProperty, value);
        }

        public static int GetRepeatCount(BindableObject b)
        {
            return (int) b.GetValue(RepeatCountProperty);
        }

        public static void SetIsLoading(BindableObject b, bool value)
        {
            b.SetValue(IsLoadingProperty, value);
        }

        public static bool GetIsLoading(BindableObject b)
        {
            return (bool) b.GetValue(IsLoadingProperty);
        }

        public static void SetLoadingTemplate(BindableObject b, DataTemplate value)
        {
            b.SetValue(LoadingTemplateProperty, value);
        }

        public static DataTemplate GetLoadingTemplate(BindableObject b)
        {
            return (DataTemplate) b.GetValue(LoadingTemplateProperty);
        }

        

        // TODO: Figure out if a selector makes sense...
        public static void SetLoadingTemplateSelector(BindableObject b, DataTemplateSelector value)
        {
            b.SetValue(LoadingTemplateSelectorProperty, value);
        }

        public static DataTemplateSelector GetLoadingTemplateSelector(BindableObject b)
        {
            return (DataTemplateSelector) b.GetValue(LoadingTemplateSelectorProperty);
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

        private static void OnIsEmptyChanged(BindableObject bindable, bool oldValue, bool newValue)
        {
            if (oldValue != newValue && newValue)
            {
                GetEmptyLayoutController(bindable).SwitchToEmptyTemplate();
            }
            else if (oldValue != newValue && !newValue)
            {
                GetEmptyLayoutController(bindable).SwitchToContent();
            }
        }

        static LoadingLayoutController GetLoadingLayoutController(BindableObject b)
        {
            return (LoadingLayoutController) b.GetValue(LoadingLayoutControllerProperty);
        }
        static EmptyLayoutController GetEmptyLayoutController(BindableObject b)
        {
            return (EmptyLayoutController)b.GetValue(EmptyLayoutControllerProperty);
        }

        static void SetLoadingLayoutController(BindableObject b, LoadingLayoutController value)
        {
            b.SetValue(LoadingLayoutControllerProperty, value);
        }
        static void SetEmptyLayoutController(BindableObject b, EmptyLayoutController value)
        {
            b.SetValue(EmptyLayoutControllerProperty, value);
        }

        static void OnControllerChanged(BindableObject b, LoadingLayoutController oldC, LoadingLayoutController newC)
        {
            if (newC == null)
            {
                return;
            }

            newC.LoadingTemplate = GetLoadingTemplate(b);
            newC.RepeatCount = GetRepeatCount(b);

            // TODO: Figure out if a selector makes sense...
            newC.LoadingTemplateSelector = GetLoadingTemplateSelector(b);
        }

        static void OnEmptyControllerChanged(BindableObject b, EmptyLayoutController oldC, EmptyLayoutController newC)
        {
            if (newC == null)
            {
                return;
            }

            newC.EmptyTemplate = GetEmptyTemplate(b);

            // TODO: Figure out if a selector makes sense...
        }
    }

    class EmptyLayoutController
    {
        readonly WeakReference<Layout<View>> _layoutWeakReference;
        DataTemplate _emptyTemplate;
        private bool _layoutIsGrid = false;
        private IList<View> _originalContent;

        public DataTemplate EmptyTemplate
        {
            get => _emptyTemplate;
            set => SetEmptyTemplate(value);
        }

        


        public EmptyLayoutController(Layout<View> layout, int repeatCount)
        {
            _layoutWeakReference = new WeakReference<Layout<View>>(layout);
        }

        void SetEmptyTemplate(DataTemplate emptyTemplate)
        {
            if (emptyTemplate is DataTemplateSelector)
            {
                throw new NotSupportedException(
                    $"You are using an instance of {nameof(DataTemplateSelector)} to set the {nameof(LoadingLayout)}.{LoadingLayout.EmptyTemplateProperty.PropertyName} property. Please provide a DataTemplate instead.");
            }

            _emptyTemplate = emptyTemplate;
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

        public void SwitchToEmptyTemplate()
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

            if (layout is Grid)
            {
                layout.Children.Add(new StackLayout());
                _layoutIsGrid = true;
            }
            else
            {
                layout.Children.Add(CreateItemView(layout));
            }

           
        }

        /// <summary>
        /// Expand the LoadingDataTemplate or use the template selector.
        /// </summary>
        /// <returns>The item view.</returns>
        View CreateItemView(Layout<View> layout)
        {
            return CreateItemView((_emptyTemplate));
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
                var view = (View) dataTemplate.CreateContent();
                return view;
            }

            return new Label() {Text = "No result found..."};
        }
    }

    class LoadingLayoutController
    {
        readonly WeakReference<Layout<View>> _layoutWeakReference;
        DataTemplate _loadingTemplate;
        int _repeatCount;
        private bool _layoutIsGrid = false;
        DataTemplateSelector _loadingTemplateSelector; // TODO: Figure out if a selector makes sense...

        private IList<View> _originalContent;

        public DataTemplate LoadingTemplate
        {
            get => _loadingTemplate;
            set => SetLoadingTemplate(value);
        }

        public int RepeatCount
        {
            get => _repeatCount;
            set => _repeatCount = value;
        }

        // TODO: Figure out if a selector makes sense...
        public DataTemplateSelector LoadingTemplateSelector
        {
            get => _loadingTemplateSelector;
            set => _loadingTemplateSelector = value;
        }

        public LoadingLayoutController(Layout<View> layout, int repeatCount)
        {
            _layoutWeakReference = new WeakReference<Layout<View>>(layout);
            _repeatCount = repeatCount <= 0 ? 1 : repeatCount;
        }

        void SetLoadingTemplate(DataTemplate loadingTemplate)
        {
            if (loadingTemplate is DataTemplateSelector)
            {
                throw new NotSupportedException(
                    $"You are using an instance of {nameof(DataTemplateSelector)} to set the {nameof(LoadingLayout)}.{LoadingLayout.LoadingTemplateProperty.PropertyName} property. Please provide a DataTemplate instead.");
            }

            _loadingTemplate = loadingTemplate;
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

            if (layout is Grid)
            {
                layout.Children.Add(new StackLayout());
                _layoutIsGrid = true;
            }

            for (int i = 0; i < _repeatCount; i++)
            {
                if (_layoutIsGrid)
                {
                    if (layout.Children[0] is StackLayout stack)
                        stack.Children.Add(CreateItemView(layout));
                }
                else
                {
                    layout.Children.Add(CreateItemView(layout));

                }
            }
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
                var view = (View) dataTemplate.CreateContent();
                return view;
            }
            else
            {
                return new Label() {Text = "Loading..."};
            }
        }
    }
}
