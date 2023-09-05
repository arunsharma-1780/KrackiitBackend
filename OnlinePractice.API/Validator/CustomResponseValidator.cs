namespace OnlinePractice.API.Validator
{
    public static class CustomResponseValidator
    {
        public static ErrorResponse? CheckModelValidation(FluentValidation.Results.ValidationResult validation)
        {
            if (!validation.IsValid)
            {
                ErrorCode errorCode = new();
                ErrorResponse err = new()
                {
                    Message = "Model validation error",
                    ErrorCode = 400
                };
                foreach (var e in validation.Errors)
                {
                    var eCode = errorCode.ErrorCodes.FirstOrDefault(x => x.Key == e.ErrorCode);
                    if (eCode != null)
                    {
                        err.Data.Add(new ValidationResult()
                        {
                            ErrorCode = eCode.Code,
                            PropertyName = e.PropertyName,
                            Message = e.ErrorMessage
                        });
                    }
                }
                return err;
            }
            return null;
        }
    }
}
