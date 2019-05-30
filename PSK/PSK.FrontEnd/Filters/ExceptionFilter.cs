﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PSK.FrontEnd.Models;

namespace PSK.FrontEnd.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is DbUpdateConcurrencyException)
            {
                context.HttpContext.Response.StatusCode = 409;
                var result = new RedirectResult("/Home/Error");
                context.Result = result;
            }
            else
            {
                context.HttpContext.Response.StatusCode = 500;
            }

            base.OnException(context);
        }
    }
}