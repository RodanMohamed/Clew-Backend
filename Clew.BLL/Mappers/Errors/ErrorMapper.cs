using Clew.Common;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.Text;
using FluentValidation.Results;

namespace Clew.BLL
{
    public class ErrorMapper : IErrorMapper
    {
        public Dictionary<string, List<Errors>> MapError(ValidationResult validationResult)
        {
            return validationResult.Errors
                   .GroupBy(r => r.PropertyName)
                   .ToDictionary(
                       g => g.Key,
                       g => g.Select(e => new Errors
                       {
                           Code = e.ErrorCode,
                           Message = e.ErrorMessage,
                       }).ToList()
                   );
        }

       
    }

   
}
