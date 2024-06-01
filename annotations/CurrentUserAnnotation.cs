using Microsoft.AspNetCore.Mvc;

public class CurrentUserAttribute : ModelBinderAttribute
{
    public CurrentUserAttribute() : base(typeof(UserModelBinder))
    {
    }
}
