namespace OnlinePractice.API.Validator
{
    public class ErrorCodeProperty
    {

        public string Key { get; set; } = string.Empty;
        public int Code { get; set; }

    }
    public class ErrorCode
    {
        public List<ErrorCodeProperty> ErrorCodes { get; set; }
        public ErrorCode()
        {
            ErrorCodes = new List<ErrorCodeProperty>(){
                  new ErrorCodeProperty() { Key = "EmailValidator", Code = 104 },
                  new ErrorCodeProperty() { Key = "GreaterThanOrEqualValidator", Code = 105 },
                  new ErrorCodeProperty() { Key = "GreaterThanValidator", Code = 106 },
                  new ErrorCodeProperty() { Key = "LengthValidator", Code = 107 },
                  new ErrorCodeProperty() { Key = "MinimumLengthValidator", Code = 108 },
                  new ErrorCodeProperty() { Key = "MaximumLengthValidator", Code = 109 },
                  new ErrorCodeProperty() { Key = "LessThanOrEqualValidator", Code = 110 },
                  new ErrorCodeProperty() { Key = "LessThanValidator", Code = 111 },
                  new ErrorCodeProperty() { Key = "NotEmptyValidator", Code = 112 },
                  new ErrorCodeProperty() { Key = "NotEqualValidator", Code = 113 },
                  new ErrorCodeProperty() { Key = "NotNullValidator", Code = 114 },
                  new ErrorCodeProperty() { Key = "PredicateValidator", Code = 115 },
                  new ErrorCodeProperty() { Key = "AsyncPredicateValidator", Code = 116 },
                  new ErrorCodeProperty() { Key = "RegularExpressionValidator", Code = 117 },
                  new ErrorCodeProperty() { Key = "EqualValidator", Code = 118 },
                  new ErrorCodeProperty() { Key = "ExactLengthValidator", Code = 119 },
                  new ErrorCodeProperty() { Key = "InclusiveBetweenValidator", Code = 120 },
                  new ErrorCodeProperty() { Key = "ExclusiveBetweenValidator", Code = 121 },
                  new ErrorCodeProperty() { Key = "CreditCardValidator", Code = 122 },
                  new ErrorCodeProperty() { Key = "ScalePrecisionValidator", Code = 123 },
                  new ErrorCodeProperty() { Key = "EmptyValidator", Code = 124 },
                  new ErrorCodeProperty() { Key = "NullValidator", Code = 125 },
                  new ErrorCodeProperty() { Key = "EnumValidator", Code = 126 },
                  new ErrorCodeProperty() { Key = "Length_Simple", Code = 127 },
                  new ErrorCodeProperty() { Key = "MinimumLength_Simple", Code = 128 },
                  new ErrorCodeProperty() { Key = "MaximumLength_Simple", Code = 129 },
                  new ErrorCodeProperty() { Key = "ExactLength_Simple", Code = 130 },
                  new ErrorCodeProperty() { Key = "InclusiveBetween_Simple", Code = 131 },
                  new ErrorCodeProperty() { Key = "_", Code = 199 }


                };
        }
    }
}
