Imports System.Globalization
Imports System.Windows.Data

Public Class MultiConverter(Of TInput, TOutput) : Implements IMultiValueConverter

    Private ReadOnly _convertFunction As Func(Of TInput(), Object, TOutput)
    Private ReadOnly _convertBackFunction As Func(Of TOutput, Object, TInput())

    Public Sub New(convertFunction As Func(Of TInput(), Object, TOutput), Optional convertBackFunction As Func(Of TOutput, Object, TInput()) = Nothing)
        _convertFunction = convertFunction
        _convertBackFunction = convertBackFunction
    End Sub

    Public Function Convert(values As Object(), targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        If _convertFunction Is Nothing Then Throw New NotImplementedException("Convert function is not defined.")
        Dim typedValues As TInput() = If(TypeOf values Is TInput(), DirectCast(values, TInput()), values.Select(Function(v) CType(v, TInput)).ToArray())

        Return _convertFunction(typedValues, parameter)
    End Function

    Public Function ConvertBack(value As Object, targetTypes As Type(), parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        If _convertBackFunction Is Nothing Then Throw New NotImplementedException("ConvertBack function is not defined.")
        Return _convertBackFunction(CType(value, TOutput), parameter).Cast(Of Object)().ToArray()
    End Function

End Class

