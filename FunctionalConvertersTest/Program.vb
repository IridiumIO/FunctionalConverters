Imports System
Imports System.Globalization
Imports System.Windows
Imports System.Windows.Data
Imports System.Windows.Markup

Imports BenchmarkDotNet.Attributes
Imports BenchmarkDotNet.Configs

Imports BenchmarkDotNet.Running

Imports FunctionalConverters

Module Program
    Sub Main(args As String())
        Dim result = BenchmarkRunner.Run(Of BenchmarkSingleConverters)()
    End Sub
End Module

<ShortRunJob>
<GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)>
<CategoriesColumn>
<MemoryDiagnoser>
Public Class BenchmarkSingleConverters

    Private baseSingleConverter As SimpleConverter
    Private baseMultiConverter As MultiConverter
    Private customSingleConverter As SingleConverter(Of Boolean, Visibility)
    Private customMultiConverter As MultiConverter(Of Boolean, Visibility)

    <GlobalSetup>
    Sub Init()
        baseSingleConverter = New SimpleConverter()
        baseMultiConverter = New MultiConverter()
        customSingleConverter = CType(New MyConverters("BooleanToVisibilityConverter").ProvideValue(Nothing), SingleConverter(Of Boolean, Visibility))
        customMultiConverter = CType(New MyConverters("CombineBooleansToVisibilityConverter").ProvideValue(Nothing), MultiConverter(Of Boolean, Visibility))


    End Sub

    <Benchmark(Baseline:=True), BenchmarkCategory("SingleConverter")>
    Function ValueConverter() As String
        Dim result = baseSingleConverter.Convert(True, Nothing, Nothing, Nothing)
        Return result.ToString()
    End Function


    <Benchmark, BenchmarkCategory("SingleConverter")>
    Function CustomValueConverter() As String
        Dim result = customSingleConverter.Convert(True, Nothing, Nothing, Nothing)
        Return result.ToString()
    End Function

    <Benchmark(Baseline:=True), BenchmarkCategory("MultiConverter")>
    Function MultiValueConverter() As String
        Dim result = baseMultiConverter.Convert(New Object() {True, True}, Nothing, Nothing, Nothing)
        Return result.ToString()
    End Function

    <Benchmark, BenchmarkCategory("MultiConverter")>
    Function CustomMultiValueConverter() As String
        Dim result = customMultiConverter.Convert(New Object() {True, True}, Nothing, Nothing, Nothing)
        Return result.ToString()
    End Function
End Class

