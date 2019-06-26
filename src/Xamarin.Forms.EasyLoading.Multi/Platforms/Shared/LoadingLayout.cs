using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.EasyLoading
{
    public static class LoadingLayout
    {
        public static readonly BindableProperty IsLoadingProperty = BindableProperty.CreateAttached("IsLoading", typeof(bool), typeof(Layout<View>), default(bool), propertyChanged: (b, o, n) => OnIsLoadingChanged(b, (bool)o, (bool)n));
        public static readonly BindableProperty LoadingTemplateProperty = BindableProperty.CreateAttached("LoadingTemplate", typeof(DataTemplate), typeof(Layout<View>), default(DataTemplate), propertyChanged: (b, o, n) => { GetLoadingLayoutController(b).LoadingStateTemplate = (DataTemplate)n; });

        public static readonly BindableProperty ErrorTemplateProperty = BindableProperty.CreateAttached("ErrorTemplate", typeof(DataTemplate), typeof(Layout<View>), default(DataTemplate), propertyChanged: (b, o, n) => { GetLoadingLayoutController(b).LoadingStateTemplate = (DataTemplate)n; });
        public static readonly BindableProperty RepeatCountProperty =
            BindableProperty.CreateAttached("RepeatCount", typeof(int), typeof(Layout<View>), defaultValue: 1,
                 propertyChanged: (b, o, n) => { GetLoadingLayoutController(b).RepeatCount = (int)n; });

        // TODO: Figure out if a selector makes sense...
        public static readonly BindableProperty LoadingTemplateSelectorProperty =
            BindableProperty.CreateAttached("LoadingTemplateSelector", typeof(DataTemplateSelector), typeof(Layout<View>), default(DataTemplateSelector),
                propertyChanged: (b, o, n) => { GetLoadingLayoutController(b).LoadingTemplateSelector = (DataTemplateSelector)n; });

        static readonly BindableProperty LoadingLayoutControllerProperty =
             BindableProperty.CreateAttached("LoadingLayoutController", typeof(LoadingLayoutController), typeof(Layout<View>), default(LoadingLayoutController),
                 defaultValueCreator: (b) => new LoadingLayoutController((Layout<View>)b, 1),
                 propertyChanged: (b, o, n) => OnControllerChanged(b, (LoadingLayoutController)o, (LoadingLayoutController)n));

        // Added to handle StateChanges for Template
        public static readonly BindableProperty LoadingStateProperty = BindableProperty.CreateAttached("LoadingState", typeof(LoadingState), typeof(Layout<View>),
            LoadingState.Success, propertyChanged: (b, o, n) => OnLoadingStateChanged(b, (LoadingState)o, (LoadingState)n));

        public static void SetRepeatCount(BindableObject b, int value)
        {
            b.SetValue(RepeatCountProperty, value);
        }

        public static int GetRepeatCount(BindableObject b)
        {
            return (int)b.GetValue(RepeatCountProperty);
        }

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

        public static void SetErrorTemplate(BindableObject b, DataTemplate value)
        {
            b.SetValue(ErrorTemplateProperty, value);
        }

        public static DataTemplate GetErrorTemplate(BindableObject b)
        {
            return (DataTemplate)b.GetValue(ErrorTemplateProperty);
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

        public static void SetLoadingState(BindableObject b, LoadingState value)
        {
            b.SetValue(LoadingStateProperty, value);
        }

        public static LoadingState GetLoadingState(BindableObject b)
        {
            return (LoadingState)b.GetValue(LoadingStateProperty);
        }

        static void OnIsLoadingChanged(BindableObject bindable, bool oldValue, bool newValue)
        {
            // Swap out the current children for the Loading Template.
            if (oldValue != newValue && newValue)
            {
                SetLoadingState(bindable, LoadingState.Loading); // Backward Compat for IsLoadingProperty
                GetLoadingLayoutController(bindable).SwitchToLoadingTemplate();
            }
            else if (oldValue != newValue && !newValue)
            {
                SetLoadingState(bindable, LoadingState.Success); // Backward Compat for IsLoadingProperty
                GetLoadingLayoutController(bindable).SwitchToContent();
            }
        }

        static void OnLoadingStateChanged(BindableObject bindable, LoadingState oldValue, LoadingState newValue)
        {
            // Swap out the current children for the Loading Template.
            if (oldValue != newValue && GetLoadingState(bindable) == LoadingState.Success)
            {
                GetLoadingLayoutController(bindable).SwitchToContent();
            }
            else if (oldValue != newValue && GetLoadingState(bindable) == LoadingState.Loading)
            {
                GetLoadingLayoutController(bindable).LoadingStateTemplate = GetLoadingTemplate(bindable);
                GetLoadingLayoutController(bindable).SwitchToLoadingTemplate();
            }
            else if (oldValue != newValue && GetLoadingState(bindable) == LoadingState.Error)
            {
                GetLoadingLayoutController(bindable).LoadingStateTemplate = GetErrorTemplate(bindable);
                GetLoadingLayoutController(bindable).SwitchToLoadingTemplate();
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
            // Use one State Template in controller to reduce workload for adding more states
            switch (GetLoadingState(b))
            {
                case LoadingState.Error:
                    newC.LoadingStateTemplate = GetErrorTemplate(b);
                    break;
                default:
                    newC.LoadingStateTemplate = GetLoadingTemplate(b);
                    break;
            }
            newC.LoadingState = GetLoadingState(b);
            newC.RepeatCount = GetRepeatCount(b);

            // TODO: Figure out if a selector makes sense...
            newC.LoadingTemplateSelector = GetLoadingTemplateSelector(b);
        }
    }

    class LoadingLayoutController
    {
        readonly WeakReference<Layout<View>> _layoutWeakReference;
        DataTemplate _loadingStateTemplate;
        int _repeatCount;
        LoadingState _loadingState;
        private bool _layoutIsGrid = false;
        DataTemplateSelector _loadingTemplateSelector; // TODO: Figure out if a selector makes sense...

        private IList<View> _originalContent;

        public DataTemplate LoadingStateTemplate { get => _loadingStateTemplate; set => SetLoadingStateTemplate(value); }
        public int RepeatCount
        {
            get => _repeatCount;
            set => _repeatCount = value;
        }
        public LoadingState LoadingState
        {
            get => _loadingState;
            set => _loadingState = value;
        }

        // TODO: Figure out if a selector makes sense...
        public DataTemplateSelector LoadingTemplateSelector { get => _loadingTemplateSelector; set => _loadingTemplateSelector = value; }

        public LoadingLayoutController(Layout<View> layout, int repeatCount)
        {
            _layoutWeakReference = new WeakReference<Layout<View>>(layout);
            _repeatCount = repeatCount <= 0 ? 1 : repeatCount;
        }

        void SetLoadingStateTemplate(DataTemplate loadingStateTemplate)
        {
            if (loadingStateTemplate is DataTemplateSelector)
            {
                throw new NotSupportedException($"You are using an instance of {nameof(DataTemplateSelector)} to set the {nameof(LoadingLayout)}.{LoadingLayout.LoadingTemplateProperty.PropertyName} property. Please provide a DataTemplate instead.");
            }

            _loadingStateTemplate = loadingStateTemplate;
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
            if (_originalContent == null)
            {
                _originalContent = new List<View>();

                foreach (var item in layout.Children)
                    _originalContent.Add(item);
            }
            // Add the loading template.
            layout.Children.Clear();
            if (_loadingState == LoadingState.Loading)
            {
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
            else if (_loadingState == LoadingState.Error)
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
            return CreateItemView(_loadingStateTemplate ?? _loadingTemplateSelector?.SelectTemplate(_loadingState, layout)); // Set Item to state so user can use selector instead
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
