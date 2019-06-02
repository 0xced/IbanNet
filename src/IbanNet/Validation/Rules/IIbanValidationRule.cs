﻿namespace IbanNet.Validation.Rules
{
	/// <summary>
	/// Describes a validation rule for IBAN.
	/// </summary>
	public interface IIbanValidationRule
	{
		/// <summary>
		/// Validates the IBAN against this rule.
		/// </summary>
		/// <param name="context">The validation context.</param>
		/// <param name="iban">The IBAN to validate.</param>
		void Validate(ValidationRuleContext context, string iban);
	}
}
