﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using IbanNet.Registry;
using IbanNet.Validation.Results;
using NUnit.Framework;

namespace IbanNet.FluentValidation
{
	[TestFixture]
	internal class IntegrationTests : IbanTestFixture
	{
		private TestModelValidator _sut;
		private TestModel _testModel;

		public override void SetUp()
		{
			base.SetUp();

			_sut = new TestModelValidator(new IbanValidator());

			_testModel = new TestModel();
		}

		[TestCase("PL611090101400000712198128741")]
		[TestCase("AE07033123456789012345")]
		public void Given_a_model_with_invalid_iban_when_validating_should_contain_validation_errors(string attemptedIbanValue)
		{
			_testModel.BankAccountNumber = attemptedIbanValue;

			const string expectedFormattedPropertyName = "Bank Account Number";
			var expectedValidationFailure = new ValidationFailure(nameof(_testModel.BankAccountNumber), $"'{expectedFormattedPropertyName}' is not a valid IBAN.")
			{
				AttemptedValue = attemptedIbanValue,
				ErrorCode = nameof(FluentIbanValidator),
				FormattedMessageArguments = new object[0],
				FormattedMessagePlaceholderValues = new Dictionary<string, object>
				{
					{ "PropertyName", expectedFormattedPropertyName },
					{ "PropertyValue", attemptedIbanValue },
					{ "Error", new InvalidLengthResult() }
				}
			};

			// Act
			var actual = _sut.Validate(_testModel);

			// Assert
			actual.IsValid.Should().BeFalse("because one validation error should have occurred");
			actual.Errors.Should()
				.HaveCount(1, "because one validation error should have occurred")
				.And.Subject.Single()
				.Should().BeEquivalentTo(expectedValidationFailure);
		}

		[TestCase("PL61109010140000071219812874")]
		[TestCase("AE070331234567890123456")]
		public void Given_a_model_with_iban_when_validating_should_not_contain_validation_errors(string attemptedIbanValue)
		{
			_testModel.BankAccountNumber = attemptedIbanValue;

			// Act
			var actual = _sut.Validate(_testModel);

			// Assert
			actual.IsValid.Should().BeTrue("because no validation errors should have occurred");
		}

		private class TestModelValidator : AbstractValidator<TestModel>
		{
			public TestModelValidator(IIbanValidator ibanValidator)
			{
				RuleFor(x => x.BankAccountNumber).Iban(ibanValidator);
			}
		}
	}
}
