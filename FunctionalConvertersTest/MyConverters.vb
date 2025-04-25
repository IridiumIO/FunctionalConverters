Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data

Imports FunctionalConverters

Public Class MyConverters : Inherits ExtensibleConverter

    Public Sub New(converterFunctionName As String)
        MyBase.New(converterFunctionName)

    End Sub


    Public Shared Function BooleanToVisibilityConverter() As SingleConverter(Of Boolean, Visibility)

        Return CreateConverter(
            convertFunction:=Function(input As Boolean) If(input, Visibility.Visible, Visibility.Collapsed),
            convertBackFunction:=Function(input As Visibility) input = Visibility.Visible
            )

    End Function


    Public Shared Function YesNoToBooleanConverter() As SingleConverter(Of String, Boolean)

        Dim convert = Function(input As String, parameter As Object)
                          If parameter IsNot Nothing AndAlso parameter.ToString() = "CaseSensitive" Then Return input = "Yes"
                          Return input.Equals("Yes", StringComparison.OrdinalIgnoreCase)
                      End Function

        Dim convertBack = Function(output As Boolean, parameter As Object)
                              Return If(output, "Yes", "No")
                          End Function

        Return CreateConverter(convert, convertBack)

    End Function

    Public Shared Function CombineBooleansToVisibilityConverter() As MultiConverter(Of Boolean, Visibility)
        Return CreateMultiConverter(Of Boolean, Visibility)(
        convertFunction:=Function(values As Boolean())
                             Dim firstBoolean = values(0)
                             Dim secondBoolean = values(1)
                             Dim result = firstBoolean AndAlso secondBoolean
                             Return If(result, Visibility.Visible, Visibility.Collapsed)
                         End Function,
        convertBackFunction:=Nothing ' Optional: Define if needed
    )
    End Function

    Public Shared Function combinestringsconverter() As MultiConverter(Of String, String)
        Return New MultiConverter(Of String, String)(
            convertFunction:=Function(values As Object(), parameter As Object)
                                 Dim firststring = TryCast(values(0), String)
                                 Dim secondstring = TryCast(values(1), String)
                                 Return $"{firststring} {secondstring}"
                             End Function,
            convertBackFunction:=Nothing ' optional: define if needed
        )
    End Function


End Class


Public Class SimpleConverter : Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Return If(value, Visibility.Visible, Visibility.Collapsed)
    End Function
    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return value = Visibility.Visible
    End Function
End Class


Public Class MultiConverter : Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim firstBoolean = values(0)
        Dim secondBoolean = values(1)
        Dim result = firstBoolean AndAlso secondBoolean
        Return If(result, Visibility.Visible, Visibility.Collapsed)
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
