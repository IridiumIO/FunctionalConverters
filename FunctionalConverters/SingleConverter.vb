Imports System.Globalization
Imports System.Windows.Data

Public Class SingleConverter(Of TInput, TOutput) : Implements IValueConverter

    Private ReadOnly _convertFunction As Func(Of TInput, Object, TOutput)
    Private ReadOnly _convertBackFunction As Func(Of TOutput, Object, TInput)

    Public Sub New(convertFunction As Func(Of TInput, Object, TOutput), Optional convertBackFunction As Func(Of TOutput, Object, TInput) = Nothing)
        _convertFunction = convertFunction
        _convertBackFunction = convertBackFunction
    End Sub


    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If _convertFunction Is Nothing Then Throw New NotImplementedException("Convert function is not defined.")
        Dim input As TInput = DirectCast(value, TInput)
        Return _convertFunction(input, parameter)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If _convertBackFunction Is Nothing Then Throw New NotImplementedException("ConvertBack function is not defined.")
        Dim output As TOutput = DirectCast(value, TOutput)
        Return _convertBackFunction(output, parameter)
    End Function

End Class

