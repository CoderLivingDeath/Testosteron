using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testosteron.Services;

namespace Testosteron.tests;

[TestClass]
public class ResultTests
{
    #region Result Struct Tests

    [TestMethod]
    public void CreateSuccess_WithMessage_ReturnsSuccessResult()
    {
        var result = Result.CreateSuccess("Operation completed");

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Operation completed", result.Message);
        Assert.IsNull(result.Errors);
    }

    [TestMethod]
    public void CreateSuccess_WithDefaultMessage_SetsEmptyMessage()
    {
        var result = Result.CreateSuccess("");

        Assert.IsTrue(result.Success);
        Assert.AreEqual("", result.Message);
    }

    [TestMethod]
    public void CreateFailure_WithMessage_ReturnsFailureResult()
    {
        var result = Result.CreateFailure("Operation failed");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Operation failed", result.Message);
        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(0, result.Errors.Length);
    }

    [TestMethod]
    public void CreateFailure_WithErrors_ReturnsFailureWithErrors()
    {
        var errors = new[] { "Error 1", "Error 2" };
        var result = Result.CreateFailure("Operation failed", errors);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Operation failed", result.Message);
        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(2, result.Errors.Length);
        CollectionAssert.AreEqual(errors, result.Errors);
    }

    [TestMethod]
    public void CreateFailure_WithNullErrors_ReturnsEmptyArray()
    {
        var result = Result.CreateFailure("Operation failed", null);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(0, result.Errors.Length);
    }

    [TestMethod]
    public void ErrorsFromException_WithMessage_AddsError()
    {
        var result = Result.CreateSuccess("test");
        var exception = new Exception("Test exception message");

        result.ErrorsFromException(exception);

        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(1, result.Errors.Length);
        Assert.AreEqual("Test exception message", result.Errors[0]);
    }

    [TestMethod]
    public void ErrorsFromException_WithInnerException_AddsBothErrors()
    {
        var result = Result.CreateSuccess("test");
        var innerException = new Exception("Inner exception message");
        var exception = new Exception("Outer exception message", innerException);

        result.ErrorsFromException(exception);

        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(2, result.Errors.Length);
        Assert.AreEqual("Outer exception message", result.Errors[0]);
        Assert.AreEqual("Inner exception message", result.Errors[1]);
    }

    [TestMethod]
    public void ErrorsFromException_WhenErrorsExist_ConcatenatesMessages()
    {
        var result = Result.CreateFailure("test", new[] { "Existing error" });
        var exception = new Exception("New exception");

        result.ErrorsFromException(exception);

        Assert.IsNotNull(result.Errors);
        Assert.HasCount(2, result.Errors);
        CollectionAssert.AreEqual(new[] { "Existing error", "New exception" }, result.Errors);
    }

    [TestMethod]
    public void ErrorsFromException_WithNullMessage_HandlesGracefully()
    {
        var result = Result.CreateSuccess("test");
        var exception = new Exception(null);

        result.ErrorsFromException(exception);

        Assert.IsNotNull(result.Errors);
        Assert.HasCount(1, result.Errors);
    }

    [TestMethod]
    public void Constructor_WithValidParameters_InitializesAllFields()
    {
        var errors = new[] { "Error 1" };
        var result = new Result(true, errors, "Test message");

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Test message", result.Message);
        CollectionAssert.AreEqual(errors, result.Errors);
    }

    #endregion

    #region Result<T> Struct Tests

    [TestMethod]
    public void CreateSuccessGeneric_WithValue_ReturnsSuccessResult()
    {
        const string value = "Test Value";
        var result = Result<string>.CreateSuccess(value, "Success message");

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Success message", result.Message);
        Assert.AreEqual(value, result.Value);
        Assert.IsNull(result.Errors);
    }

    [TestMethod]
    public void CreateSuccessGeneric_WithDefaultValue_InitializesCorrectly()
    {
        var result = Result<object>.CreateSuccess(null!, "");

        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Value);
    }

    [TestMethod]
    public void CreateFailureGeneric_WithValue_ReturnsFailureWithValue()
    {
        const string value = "Test Value";
        var result = Result<string>.CreateFailure(value, "Failed message");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Failed message", result.Message);
        Assert.AreEqual(value, result.Value);
    }

    [TestMethod]
    public void CreateFailureGeneric_WithErrors_ReturnsFailureWithErrors()
    {
        var errors = new[] { "Error 1", "Error 2" };
        var result = Result<string>.CreateFailure("default", "Failed", errors);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Failed", result.Message);
        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(2, result.Errors.Length);
        CollectionAssert.AreEqual(errors, result.Errors);
    }

    [TestMethod]
    public void CreateFailureGeneric_WithNullErrors_ReturnsEmptyArray()
    {
        var result = Result<string>.CreateFailure("default", "Failed", null);

        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(0, result.Errors.Length);
    }

    [TestMethod]
    public void ErrorsFromExceptionGeneric_WithMessage_AddsError()
    {
        var result = Result<string>.CreateSuccess("test");
        var exception = new Exception("Test exception");

        result.ErrorsFromException(exception);

        Assert.IsNotNull(result.Errors);
        Assert.HasCount(1, result.Errors);
        Assert.AreEqual("Test exception", result.Errors[0]);
    }

    [TestMethod]
    public void ErrorsFromExceptionGeneric_WhenErrorsExist_ConcatenatesMessages()
    {
        var result = Result<string>.CreateFailure("test", "", ["Existing"]);
        var exception = new Exception("New");

        result.ErrorsFromException(exception);

        Assert.IsNotNull(result.Errors);
        Assert.HasCount(2, result.Errors);
        CollectionAssert.AreEqual(new[] { "Existing", "New" }, result.Errors);
    }

    [TestMethod]
    public void ConstructorGeneric_WithValidParameters_InitializesAllFields()
    {
        const string value = "Test Value";
        var errors = new[] { "Error 1" };
        var result = new Result<string>(value, true, errors, "Message");

        Assert.IsTrue(result.Success);
        Assert.AreEqual(value, result.Value);
        Assert.AreEqual("Message", result.Message);
        CollectionAssert.AreEqual(errors, result.Errors);
    }

    #endregion

    #region IResult Interface Tests

    [TestMethod]
    public void IResult_GetSuccess_ReturnsCorrectValue()
    {
        var successResult = Result.CreateSuccess("msg");
        var failureResult = Result.CreateFailure("msg");

        Assert.IsTrue(((IResult)successResult).Success);
        Assert.IsFalse(((IResult)failureResult).Success);
    }

    [TestMethod]
    public void IResult_GetMessage_ReturnsCorrectValue()
    {
        var result = Result.CreateSuccess("Test message");

        Assert.AreEqual("Test message", ((IResult)result).Message);
    }

    [TestMethod]
    public void IResult_GetErrors_ReturnsCorrectValue()
    {
        var errors = new[] { "Error 1" };
        var result = Result.CreateFailure("msg", errors);

        Assert.IsNotNull(((IResult)result).Errors);
        CollectionAssert.AreEqual(errors, ((IResult)result).Errors!);
    }

    [TestMethod]
    public void IResultGeneric_GetValue_ReturnsCorrectValue()
    {
        const int value = 42;
        var result = Result<int>.CreateSuccess(value);

        Assert.AreEqual(value, ((IResult<int>)result).Value);
    }

    #endregion
}
