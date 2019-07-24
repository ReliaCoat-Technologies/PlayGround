using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EnumRadioButtonDesigner.Views
{
    public partial class EnumRadioButtonSelector : Selector, INotifyPropertyChanged
    {
        #region DependencyProperties
        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumRadioButtonSelector), new PropertyMetadata(typeof(Enum)));
        #endregion

        #region Fields
        private ObservableCollection<EnumRadioButtonContext> _enumeratorValues;
        #endregion

        #region Delegates
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public Type EnumType
        {
            get { return getValue(); }
            set { SetValue(EnumTypeProperty, value); raisePropertyChanged(nameof(EnumType)); }
        }
        #endregion

        #region Constructor
        public EnumRadioButtonSelector()
        {
            DataContext = this;
            Loaded += onUiElementLoaded;

            InitializeComponent();
        }
        #endregion

        #region Methods
        private void onUiElementLoaded(object sender, RoutedEventArgs e)
        {
            populateRadioButtons();
        }

        private void setValue(object value)
        {
            SetValue(SelectedValueProperty, value);
            raisePropertyChanged(nameof(SelectedValue));
        }

        private Type getValue()
        {
            var value = (Type)GetValue(EnumTypeProperty);

            if (value.BaseType?.IsEnum ?? false)
                throw new Exception("Type must be an enumerator");

            return value;
        }

        private void populateRadioButtons()
        {
            var itemSourceToSet = new ObservableCollection<EnumRadioButtonContext>();

            foreach (var item in EnumType.GetEnumValues())
                itemSourceToSet.Add(new EnumRadioButtonContext(item, EnumType));

            ItemsSource = itemSourceToSet;
        }

        public void raisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void onRadioButtonClicked(object sender, RoutedEventArgs e)
        {
            SelectedValue = sender;
        }
    }

    public class EnumRadioButtonContext
    {
        public bool isChecked { get; set; }
        public object enumValue { get; set; }
        public string enumValueName { get; set; }

        public EnumRadioButtonContext(object enumValue, Type enumType)
        {
            if (!enumValue.GetType().IsEnum)
                throw new Exception("Selected value does not match enumerator type.");

            this.enumValue = enumValue;

            var enumInfo = enumType.GetMember(enumType.GetEnumName(enumValue));

            var descriptionAttribute = enumInfo
                .First()
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .OfType<DescriptionAttribute>()
                .FirstOrDefault();

            enumValueName = descriptionAttribute == null
                ? enumValue.ToString()
                : descriptionAttribute.Description;
        }
    }
}