using FluentValidation;
using System;

namespace Cloud.$ext_safeprojectname$.Infrastructure.Base
{
    public class BaseAttributeValidator
    {
        private static Func<CascadeMode> _cascadeMode = () => ValidatorOptions.Global.CascadeMode;
		/// <summary>
		/// Sets the cascade mode for all rules within this validator.
		/// </summary>
		public static CascadeMode CascadeMode
		{
			get => _cascadeMode();
			set => _cascadeMode = () => value;
		}
	}
}
