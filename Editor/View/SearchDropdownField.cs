using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.SettingsManagement.Editor
{

    internal class SearchDropdownField : BaseField<object>, INotifyValueChanged<object>
    {
        //private object _value;
        private SearchPopupContent popup;
        private Label textElement;
        VisualElement inputContainer;
        public SearchDropdownField()
            : this(null)
        {
        }
        public SearchDropdownField(string label)
            : base(label, null)
        {

            inputContainer = this.Q(className: "unity-base-field__input");
            inputContainer.AddToClassList("unity-base-popup-field__input");
            inputContainer.AddToClassList("unity-popup-field__input");
            inputContainer.RegisterCallback<MouseDownEvent>(e => ShowPopup());

            textElement = new Label();
            textElement.AddToClassList("unity-base-popup-field__text");
            textElement.style.marginLeft = 0;
            textElement.style.marginRight = 0;
            inputContainer.Add(textElement);

            VisualElement arrow = new VisualElement();
            arrow.AddToClassList("unity-base-popup-field__arrow");
            inputContainer.Add(arrow);
            Add(inputContainer);

            this.RegisterValueChangedCallback(e =>
            {
                UpdateValue();
            });

            popup = new SearchPopupContent();
            popup.OnSelected += (item) =>
            {
                value = item;
            };
            Text = null;
        }

        public Func<object, string> FormatSelectedValueCallback;
        public Func<object, string> FormatListItemCallback { get => popup.formatListItemCallback; set => popup.formatListItemCallback = value; }
        public Action<IList> LoadItems { get => popup.loadItems; set => popup.loadItems = value; }

        public Func<object, string, bool> Filer { get => popup.filer; set => popup.filer = value; }

        public SearchPopupContent Popup => popup;



        //public new object value
        //{
        //    get => base.value;
        //    set
        //    {
        //        var oldValue = this.value;
        //        if (oldValue != value)
        //        {
        //            base.value = value;
        //            UpdateValue();
        //            using (var e = ChangeEvent<object>.GetPooled(oldValue, _value))
        //            {
        //                SendEvent(e);
        //            }
        //        }
        //    }
        //}

        public string Text
        {
            get => textElement.text;
            set => textElement.text = value;
        }

        public Label TextElement { get => textElement; }

        public override void SetValueWithoutNotify(object newValue)
        {
            base.SetValueWithoutNotify(newValue);
            UpdateValue();
        }

        void UpdateValue()
        {
            TextElement.text = ItemToString(value);
        }

        private string ItemToString(object item)
        {
            string text = null;
            if (item != null)
            {
                if (FormatSelectedValueCallback != null)
                    text = FormatSelectedValueCallback(item);
                if (FormatListItemCallback != null)
                    text = FormatListItemCallback(item);
                else
                    text = item.ToString();
            }
            return text;
        }

        private void ShowPopup()
        {
            popup.filer = Filer;

            popup.Show(inputContainer);
        }


        public new class UxmlFactory : UxmlFactory<SearchDropdownField, UxmlTraits> { }

    }
    public class SearchPopupContent : PopupWindowContent
    {
        private Vector2 size;
        private VisualElement container;
        private ToolbarSearchField searchField;
        private ListView listView;
        private bool inputChange;

        public float height = 270;
        public float minWidth = 150f;

        private IList<SearchPopupItem> list;
        public Action<IList> loadItems;
        private List<SearchPopupItem> currentList;
        public Func<object, string, bool> filer;
        public Func<object, string> formatListItemCallback;

        public Action<IList<SearchPopupItem>> loadItems2;

        public event Action<object> OnSelected;


        public SearchPopupContent()
        {
            list = new List<SearchPopupItem>();
            currentList = new List<SearchPopupItem>();
            CreateView();
        }

        IList<SearchPopupItem> List
        {
            get => list;
        }

        public ListView ListView => listView;

        public string SearchText
        {
            get => searchField.value;
            set
            {
                if (searchField.value != value)
                {
                    searchField.value = value;
                }
            }
        }


        void CreateView()
        {
            container = new VisualElement();
            var style = container.style;
            style.marginLeft = 1;
            style.marginTop = 1;
            style.marginRight = 1;
            style.marginBottom = 1;

            container.Add(new IMGUIContainer(() => { if (Event.current.type == EventType.Repaint) Update(); }));

            Toolbar toolbar = new Toolbar();
            style = toolbar.style;

            searchField = new ToolbarSearchField();
            style = searchField.style;
            style.flexGrow = 1f;
            style.minWidth = StyleKeyword.None;
            style.width = StyleKeyword.None;

            searchField.RegisterCallback<KeyDownEvent>(e =>
            {
                switch (e.keyCode)
                {
                    case KeyCode.UpArrow:
                        if (listView.selectedIndex > 0)
                        {
                            inputChange = true;
                            listView.selectedIndex--;
                            searchField.schedule.Execute(() =>
                            {
                                inputChange = false;
                            });
                        }
                        e.StopImmediatePropagation();
                        break;
                    case KeyCode.DownArrow:
                        if (listView.selectedIndex < listView.itemsSource.Count - 1 && listView.itemsSource.Count > 0)
                        {
                            inputChange = true;
                            if (listView.selectedIndex < 0)
                                listView.selectedIndex = 0;
                            else
                                listView.selectedIndex++;

                            searchField.schedule.Execute(() =>
                            {
                                inputChange = false;
                            });
                        }
                        e.StopImmediatePropagation();
                        break;
                    case KeyCode.Return:
                        if (listView.selectedItem != null)
                        {
                            var item = listView.selectedItem as SearchPopupItem;
                            ConfirmSelect(item.userData);
                        }
                        e.StopImmediatePropagation();
                        break;
                }
            });

            searchField.RegisterValueChangedCallback(e =>
            {
                if (inputChange)
                    return;
                listView.selectedIndex = -1;
                Refresh();
            });

            toolbar.Add(searchField);
            container.Add(toolbar);

            listView = new ListView();
            listView.focusable = false;
            listView.fixedItemHeight = 25;
            style = listView.style;
            style.flexGrow = 1f;


            listView.onSelectionChange += (items) =>
            {
                if (inputChange)
                    return;
                var item = items.FirstOrDefault() as SearchPopupItem;
                if (item != null)
                {
                    ConfirmSelect(item.userData);
                }
            };
            container.Add(listView);
        }

        public override Vector2 GetWindowSize() => size;

        protected virtual void LoadItems(IList<SearchPopupItem> list)
        {
            if (loadItems2 != null)
            {
                loadItems2(list);
            }
            else if (loadItems != null)
            {
                var tmp = new List<object>();
                loadItems(tmp);
                foreach (object item in tmp)
                {
                    string text = null;
                    if (formatListItemCallback != null)
                    {
                        text = ItemToString(item);
                    }
                    list.Add(new SearchPopupItem(text, item));
                }
            }
        }

        public override void OnOpen()
        {
            if (container.parent != editorWindow.rootVisualElement)
                editorWindow.rootVisualElement.Add(container);

            listView.selectedIndex = -1;
            searchField.style.width = size.x - 15;
            searchField.SetValueWithoutNotify(null);

            //Focus();
            searchField.schedule.Execute(Focus).ExecuteLater(100);
            Refresh();
            base.OnOpen();
        }
        protected virtual void Update()
        {

        }

        void Focus()
        {
            //if (editorWindow != null && editorWindow.hasFocus)
            {
                //searchField.SendEvent(FocusInEvent.GetPooled());
                //searchField.Focus();
                searchField.Q("unity-text-input").Focus();
            }
        }

        public virtual void ConfirmSelect(object userData)
        {
            if (userData != null)
            {
                OnSelected?.Invoke(userData);
            }
            editorWindow.Close();
        }


        public virtual void Refresh()
        {
            currentList.Clear();
            IEnumerable<SearchPopupItem> items = List;

            if (!string.IsNullOrEmpty(SearchText))
            {
                var parts = SearchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 0)
                {
                    if (filer != null)
                    {
                        items = items.Where(
                            item =>
                            {
                                bool isMatch = true;
                                foreach (var part in parts)
                                {
                                    if (!filer(item.userData, part))
                                    {
                                        isMatch = false;
                                    }
                                }
                                return isMatch;
                            });
                    }
                    else
                    {
                        items = items.Where(
                            item =>
                            {
                                bool isMatch = true;
                                foreach (var part in parts)
                                {
                                    if (!Filter(item, part))
                                    {
                                        isMatch = false;
                                    }
                                }
                                return isMatch;
                            });
                    }
                }
                else
                {
                    if (filer != null)
                    {
                        items = items.Where(
                            item =>
                            {
                                return filer(item.userData, string.Empty);
                            });
                    }
                }
            }
            else
            {
                if (filer != null)
                {
                    items = items.Where(
                        item =>
                        {
                            return filer(item.userData, string.Empty);
                        });
                } 
            }

            foreach (var item in items)
            {
                currentList.Add(item);
            }

            searchField.SetValueWithoutNotify(SearchText);


            if (listView.makeItem == null)
            {
                listView.makeItem = () =>
                {
                    VisualElement container = new VisualElement();
                    container.style.paddingLeft = 5;
                    container.style.justifyContent = Justify.Center;

                    Label nameLabel = new Label();
                    container.Add(nameLabel);
                    return container;
                };

                listView.bindItem = (view, index) =>
                {
                    var item = listView.itemsSource[index] as SearchPopupItem;
                    Label nameLabel = view.Q<Label>();
                    nameLabel.text = item.text;
                };
            }


            listView.itemsSource = currentList;
            listView.RefreshItems();

            if (currentList.Count == 0)
            {
                var emptyLabel = listView.Q(className: "unity-list-view__empty-label");
                if (emptyLabel != null)
                    emptyLabel.style.display = DisplayStyle.None;
            }
        }

        private string ItemToString(object item)
        {
            string text = null;
            if (item != null)
            {
                if (formatListItemCallback != null)
                    text = formatListItemCallback(item);
                else
                    text = item.ToString();
            }
            return text;
        }

        public override void OnGUI(Rect rect)
        {

        }

        public virtual bool Filter(SearchPopupItem item, string filter)
        {
            return item.Filter(filter);
        }

        public void Show(VisualElement owner)
        {
            Show(owner, height: height, minWidth: minWidth);
        }

        public void Show(VisualElement owner, float height = 270, float minWidth = 200f)
        {
            VisualElement root = owner;
            while (root.parent != null)
            {
                root = root.parent;
            }

            var style = owner.style;
            Rect rect = owner.layout;
            rect.x = style.marginLeft.value.value;
            rect.y = rect.yMax;
            rect.xMax -= style.marginRight.value.value;
            rect = owner.ChangeCoordinatesTo(root, rect);

            if (rect.width < minWidth) rect.width = minWidth;
            rect.height = height;
            Show(rect);
        }

        public void Show(Rect rect)
        {
            if (rect.width < minWidth)
                rect.width = minWidth;

            this.size = rect.size;

            List.Clear();
            LoadItems(List);

            UnityEditor.PopupWindow.Show(new Rect(rect.x, rect.y, size.x, 0), this);
        }

        public class SearchPopupItem
        {
            public string text;
            public object userData;

            public SearchPopupItem(string text, object userData)
            {
                this.text = text;
                this.userData = userData;
            }

            public virtual bool Filter(string filter)
            {
                if (text == null)
                    return false;
                return text.Contains(filter);
            }
        }
    }
}
