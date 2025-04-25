Imports System.Windows.Markup

Public MustInherit Class ExtensibleConverter : Inherits MarkupExtension

    Public Property Converter As String

    Public Sub New(Optional converterName As String = "")
        Me.Converter = converterName
    End Sub

    Private Shared ReadOnly MethodCache As New Dictionary(Of String, Reflection.MethodInfo)()

    Public Overrides Function ProvideValue(serviceProvider As IServiceProvider) As Object

        If String.IsNullOrEmpty(Converter) Then Throw New InvalidOperationException("ConverterFunctionName must be specified.")

        Dim converterMethod As Reflection.MethodInfo = Nothing
        If Not MethodCache.TryGetValue(Converter, converterMethod) Then
            converterMethod = Me.GetType().GetMethod(Converter, Reflection.BindingFlags.Static Or Reflection.BindingFlags.Public)
            If converterMethod Is Nothing Then Throw New MissingMethodException($"The converter '{Converter}' was not found in the class '{Me.GetType().Name}'.")
            MethodCache(Converter) = converterMethod
        End If

        Dim converterInstance As Object = converterMethod.Invoke(Me, Nothing)
        If converterInstance Is Nothing Then Throw New InvalidOperationException($"The method '{Converter}' did not return a valid converter instance.")

        Return converterInstance

    End Function

    Protected Shared Function CreateConverter(Of TInput, TOutput)(convertFunction As Func(Of TInput, Object, TOutput), Optional convertBackFunction As Func(Of TOutput, Object, TInput) = Nothing) As SingleConverter(Of TInput, TOutput)
        Return New SingleConverter(Of TInput, TOutput)(convertFunction, convertBackFunction)
    End Function

    Protected Shared Function CreateConverter(Of TInput, TOutput)(convertFunction As Func(Of TInput, TOutput), Optional convertBackFunction As Func(Of TOutput, TInput) = Nothing) As SingleConverter(Of TInput, TOutput)
        ' Wrap the simpler version into the parameterized version
        Dim wrappedConvert = Function(input As TInput, parameter As Object) convertFunction(input)
        Dim wrappedConvertBack = If(convertBackFunction IsNot Nothing, Function(output As TOutput, parameter As Object) convertBackFunction(output), Nothing)
        Return New SingleConverter(Of TInput, TOutput)(wrappedConvert, wrappedConvertBack)
    End Function


    Protected Shared Function CreateMultiConverter(Of TInput, TOutput)(convertFunction As Func(Of TInput(), Object, TOutput), Optional convertBackFunction As Func(Of TOutput, Object, TInput()) = Nothing) As MultiConverter(Of TInput, TOutput)
        Return New MultiConverter(Of TInput, TOutput)(convertFunction, convertBackFunction)
    End Function

    Protected Shared Function CreateMultiConverter(Of TInput, TOutput)(convertFunction As Func(Of TInput(), TOutput), Optional convertBackFunction As Func(Of TOutput, TInput()) = Nothing) As MultiConverter(Of TInput, TOutput)
        ' Wrap the simpler version into the parameterized version
        Dim wrappedConvert = Function(inputs As TInput(), parameter As Object) convertFunction(inputs)
        Dim wrappedConvertBack = If(convertBackFunction IsNot Nothing, Function(output As TOutput, parameter As Object) convertBackFunction(output), Nothing)
        Return New MultiConverter(Of TInput, TOutput)(wrappedConvert, wrappedConvertBack)
    End Function


End Class

