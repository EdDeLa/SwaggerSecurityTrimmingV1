namespace SwaggerSecurityTrimmingV1.Models.Base
{
    public abstract class ModelBaseWithId : IModelBaseWithId
    {
        public Guid Id { get; set; }
    }
}
