using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.Text;
using Clew.Common;
using FluentValidation.Results;

namespace Clew.BLL
{
    public interface IErrorMapper
    {
        Dictionary<string, List<Errors>> MapError(ValidationResult validationResult);
    }
}
