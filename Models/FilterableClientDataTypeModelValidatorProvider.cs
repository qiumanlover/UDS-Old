using UDS.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UDS.Models
{
    public class FilterableClientDataTypeModelValidatorProvider : ClientDataTypeModelValidatorProvider
    {
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var allValidators = base.GetValidators(metadata, context);
            var validators = new List<ModelValidator>();
            foreach (var v in allValidators)
            {
                if (v.GetType().Name != "NumericModelValidator")
                {
                    validators.Add(v);
                }
                else
                {
                    NumericAttribute attribute = new NumericAttribute { ErrorMessage = Resources.FieldMustBeNumeric };
                    DataAnnotationsModelValidator validator = new DataAnnotationsModelValidator(metadata, context, attribute);
                    validators.Add(validator);
                }
            }
            return validators;
        }
    }
    internal class NumericAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            return true;
        }
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule { ValidationType = "number", ErrorMessage = this.FormatErrorMessage(metadata.DisplayName) };
        }
    }
}