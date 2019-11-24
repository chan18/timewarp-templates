﻿namespace TimeWarp.Blazor.Client.ApplicationFeature
{
  using TimeWarp.Blazor.Client.Features.Base.Components;

  public partial class AccountMenu
  {
    protected void ButtonClick() => Show = !Show;
    protected bool Show { get; set; }
    protected string ShowCssClass => Show ? "show" : null;
  }
}
