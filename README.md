# FunctionalConverters

Functional Converters is a library that allows you to define converters using a single class, with converters defined as functions rather than their own classes. 

Designed for WPF, may work with UWP and MAUI, but not tested yet.

```csharp
//Converts a boolean to visibility, and provides a ConvertBack method

```
&nbsp;

&nbsp;


## Class Setup
All custom converters inherit from the ExtensibleConverter class. You only need one class, but you can define more if you wish.

```csharp

class MyConverters(string converterName) : ExtensibleConverter(converterName)
{
    public static SingleConverter<bool, Visibility> BooleanToVisibility()
    {
        return CreateConverter<bool, Visibility>(
            convertFunction: input => input ? Visibility.Visible : Visibility.Collapsed,
            convertBackFunction: input => input == Visibility.Visible
        );
    }

    //Define more converters here
}
```
Note that you do *not* need to define a resource key (e.g. ` <local:BooleanToVisibility x:Key="BooleanToVisibility"/>`) in your XAML anymore!
Instead, you can use the converter directly in your XAML like this:
```xml
<Grid Visibility="{Binding IsVisible, Converter={local:MyConverters BooleanToVisibility}}" />
```

Note the string parameter passed into the `MyConverters` constructor. It makes the XAML cleaner, but if you *don't* define this parameter, you can still use your converters in XAML like this:
```xml
<Grid Visibility="{Binding IsVisible, Converter={local:MyConverters Converter='BooleanToVisibility'}}" />
```


## Extension Setup
Each converter is defined as a **static** method, returning a type of either:
- `SingleConverter<TInput, TOutput>`: For a single input and output
- `MultiConverter<TInput, TOutput>`: For multiple inputs and a single output. Note that `TInput` is implicitly converted to an array of the input type. 

&nbsp;


### Example 1: `SingleConverter` using lambda syntax
Converters can entirely be defined using lambda syntax, which is the most concise way to define them. The `convertBackFunction` is also optional, so you can omit this entirely in one-way conversions.
```csharp
public static SingleConverter<bool, Visibility> BooleanToVisibility()
{
    return new SingleConverter<bool, Visibility>(
        (input, p) => input ? Visibility.Visible : Visibility.Collapsed
    );
}
```

&nbsp;


### Example 2: `SingleConverter` using more verbose syntax
Of course, if you prefer more space for readability, you can define your converters as such:
```csharp
public static SingleConverter<bool, Visibility> BooleanToVisibilityConverter()
{

    var convertFunction = (bool input, object parameter) =>
    {   
        return input ? Visibility.Visible : Visibility.Collapsed;
    };

    var convertBackFunction = (Visibility input, object parameter) =>
    {     
        return input == Visibility.Visible;
    };

    return new SingleConverter<bool, Visibility>(convertFunction, convertBackFunction);
}

```
&nbsp;



### Example 3: Using a MultiConverter
Multiconverters are defined in the same way as single converters.

**However**, note that while the `MultiConverter` class is defined as `MultiConverter<TInput, TOutput>`, the `TInput` type is implicitly converted to `TInput[]` later.

This means that for a boolean multiconverter, the `convertFunction` will take in a `bool[]` parameter, while the `MultiConverter` class itself is still defined as `MultiConverter<bool, TOutput>`, without the `[]`.
```csharp
public static MultiConverter<bool, Visibility> MultiBoolToVisibility()
{
    var convertFunction = (bool[] input, object parameter) =>
    {
        return (input[0] && input[1]) ? Visibility.Visible : Visibility.Collapsed;
    };

    return new MultiConverter<bool, Visibility>(
        convertFunction, convertBackFunction: null
    );

}

```
&nbsp;

&nbsp;

## Helper Method
Notice in all the above examples, you have to include the `object parameter` in the function signature. 
FunctionalConverters provides a helper `CreateConverter` method that can abstract some of this away and make it even simpler to define your converters.

```csharp
//Not using the helper method
public static SingleConverter<bool, Visibility> BooleanToVisibility()
{ 
    return new SingleConverter<bool, Visibility>(
        convertFunction: (input, parameter) => input ? Visibility.Visible : Visibility.Collapsed,
        convertBackFunction: (output, parameter) => output == Visibility.Visible
    );
}
//Using the CreateConverter helper method
public static SingleConverter<bool, Visibility> BooleanToVisibility()
{
    return CreateConverter<bool, Visibility>(
        convertFunction: input => input ? Visibility.Visible : Visibility.Collapsed,
        convertBackFunction: output => output == Visibility.Visible
    );
}

```

The `CreateConverter` method has overloads for `SingleConverter` and `MultiConverter`:
&nbsp;

&nbsp;
## Full Example Implementation
```csharp
public class MyConverters(string converterName) : ExtensibleConverter(converterName)
{
   
    //Boolean to Visibility two-way converter
    public static SingleConverter<bool, Visibility> BooleanToVisibility()
    {
        return CreateConverter<bool, Visibility>(
            convertFunction: input => input ? Visibility.Visible : Visibility.Collapsed,
            convertBackFunction: input => input == Visibility.Visible
        );
    }

    //Multiple Boolean to Visibility one-way converter
    public static MultiConverter<bool, Visibility> MultiBoolToVisibility()
    {
        return CreateConverter<bool, Visibility>(
            bool[] input => (input[0] && input[1]) ? Visibility.Visible : Visibility.Collapsed
        );
    }

    //String to Boolean two-way converter
    public static SingleConverter<string, bool> YesNoToBooleanConverter()
    {
        var convert = (string input, object parameter) =>
        {
            if (parameter != null && parameter.ToString() == "CaseSensitive")
                return input == "Yes";
            return string.Equals(input, "Yes", StringComparison.OrdinalIgnoreCase);
        };

        var convertBack = (bool input, object parameter) =>
        {
            return input ? "Yes" : "No";
        };

        return new SingleConverter<string, bool>(convert, convertBack);
    }

}
```


## Performance
FunctionalConverters uses some reflection to find the converter methods in the `MarkupExtension.ProvideValue` method. 
I am not totally familiar with the MarkupExtension class, but I *think* this is only called once per converter. I cache the returned methods just in case, but if I'm correct the performance hit is small and once-off only.

As far as the actual converter performance once initialised, here's the results of benchmarking with BenchmarkDotNet:

```json
| Method                    | Mean     | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |---------:|---------:|---------:|------:|-------:|----------:|------------:|
| IValueConverter*          | 23.68 ns | 1.453 ns | 0.080 ns |  1.00 | 0.0029 |      48 B |        1.00 |
| SingleConverter           | 23.66 ns | 0.808 ns | 0.044 ns |  1.00 | 0.0029 |      48 B |        1.00 |
|                           |          |          |          |       |        |           |
| IMultiValueConverter*     | 51.78 ns | 3.030 ns | 0.166 ns |  1.00 | 0.0081 |     136 B |        1.00 |
| MultiConverter            | 73.94 ns | 9.355 ns | 0.513 ns |  1.43 | 0.0114 |     192 B |        1.41 |
```

The SingleConverter is roughly on par with the baseline `IValueConverter` implementation. 
The MultiConverter is slower, but this is likely due to the fact that it has to convert the input to an array first. While the relative performance is worse by ~43%, the absolute performance is still very fast at ~74ns. 

If anyone knows more about how the MarkupExtension class works, please let me know if this is a performance concern. I could implement additional caching or switch to delegates instead of reflection but my naive testing didn't identify any real performance changes with those methods (this may change if there are many converters however). 

