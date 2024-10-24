namespace ClassicComponent.Maui.CustomViews;

public class SBButton : Button
{
	
	/// <summary>
	/// Toggle Flash Binding property
	/// </summary>
	public static readonly BindableProperty IsSelectedProperty =
						BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(SBButton), false, propertyChanged: SelectionPropertyChanged);

	private static void SelectionPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
	{
		if (newvalue is bool newValue)
			(bindable as SBButton)?.ButtonEnabled(newValue);
	}

	/// <summary>
	/// Toggle Selected property.
	/// </summary>
	public bool IsSelected
	{
		get => (bool)GetValue(IsSelectedProperty);
		set => SetValue(IsSelectedProperty, value);
	}

	private void ButtonEnabled(bool value)
	{
		if (value)
		{
			BackgroundColor = Colors.LightBlue;
		}
		else
		{
			BackgroundColor = Colors.WhiteSmoke;
		}
	}
}