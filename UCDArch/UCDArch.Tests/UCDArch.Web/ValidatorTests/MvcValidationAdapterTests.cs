using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using UCDArch.Testing;
using UCDArch.Web.Validator;
using Validator = UCDArch.Core.DataAnnotationsValidator.CommonValidatorAdapter.Validator;
using Xunit;

namespace UCDArch.Tests.UCDArch.Web.ValidatorTests
{
    public class MvcValidationAdapterTests
    {
        public MvcValidationAdapterTests()
        {
            ServiceLocatorInitializer.Init();
        }

        /// <summary>
        /// Determines whether this instance [can validate object].
        /// </summary>
        [Fact]
        public void CanValidateObject()
        {
            var validator = new Validator();

            var invalidObject = new SomeObject
            {
                LastName = "",
                FirstName = "ThisFirstNameIsTooLong",
                Street = null
            };
            Assert.False(validator.IsValid(invalidObject));

            var validObject = new SomeObject
            {
                LastName = "Last",
                FirstName = "First",
                Street = "SomeStreet"
            };
            Assert.True(validator.IsValid(validObject));
        }

        /// <summary>
        /// Determines whether this instance [can transfer validation results to model state].
        /// </summary>
        [Fact]
        public void CanTransferValidationResultsToModelState()
        {
            var validator = new Validator();

            var invalidObject = new SomeObject
            {
                LastName = null,
                FirstName = "ThisFirstNameIsTooLong",
                Street = " ",
                MiddleName = "valid"
            };
            Assert.False(validator.IsValid(invalidObject));

            var results = validator.ValidationResultsFor(invalidObject);
            Assert.NotNull(results);
            Assert.Equal(3, results.Count);

            ModelStateDictionary modelState = new ModelStateDictionary();
            Assert.NotNull(modelState);
            Assert.Equal(0, modelState.Values.Count);
            MvcValidationAdapter.TransferValidationMessagesTo(modelState, validator.ValidationResultsFor(invalidObject));

            Assert.Equal(3, modelState.Values.Count);

            var resultsList = new List<string>();
            foreach (var result in modelState.Values)
            {
                foreach (var errs in result.Errors)
                {
                    resultsList.Add(errs.ErrorMessage);
                }
            }
            var errors = new[]
                             {
                                 "Dude...the name please!!",
                                 "The Street field is required.",
                                 "The field FirstName must be a string with a maximum length of 10."
                             };

            Assert.Equal(resultsList.Count, errors.Length);
            foreach (var error in errors)
            {
                Assert.True(resultsList.Contains(error), "Expected error \"" + error + "\" not found");
            }


        }


        #region Nested type: SomeObject

        /// <summary>
        /// A class to validate against
        /// </summary>
        private class SomeObject
        {
            [Required(ErrorMessage = "Dude...the name please!!")]
            public string LastName { get; set; }

            [Required]
            public string Street { get; set; }

            [StringLength(10)]
            public string FirstName { get; set; }

            [StringLength(10)]
            public string MiddleName { get; set; }
        }

        #endregion
    }
}