﻿// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace BootstrapBlazor.Components;

/// <summary>
/// 蓝牙设备 BT BatteryLevel 组件
/// </summary>
public partial class BatteryLevel : IAsyncDisposable
{
    [Inject] IJSRuntime? JS { get; set; }
    private IJSObjectReference? module;
    private DotNetObjectReference<BatteryLevel>? InstanceBatteryLevel { get; set; }

    /// <summary>
    /// UI界面元素的引用对象
    /// </summary>
    public ElementReference BatteryLevelElement { get; set; }

    /// <summary>
    /// 获得/设置 蓝牙设备
    /// </summary>
    [Parameter]
    public BluetoothDevice? Device { get; set; } = new BluetoothDevice();

    /// <summary>
    /// 获得/设置 状态更新回调方法
    /// </summary>
    [Parameter]
    public Func<BluetoothDevice, Task>? OnUpdateStatus { get; set; }

    /// <summary>
    /// 获得/设置 数值更新回调方法
    /// </summary>
    [Parameter]
    public Func<decimal, Task>? OnUpdateValue { get; set; }

    /// <summary>
    /// 获得/设置 错误更新回调方法
    /// </summary>
    [Parameter]
    public Func<string, Task>? OnUpdateError { get; set; }

    /// <summary>
    /// 获得/设置 显示log
    /// </summary>
    [Parameter]
    public bool Debug { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                Device??= new BluetoothDevice();
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.Printer/lib/batterylevel/app.js");
                InstanceBatteryLevel = DotNetObjectReference.Create(this);
                //await module.InvokeVoidAsync("getBatteryLevel", InstanceBatteryLevel, BatteryLevelElement);
            }
        }
        catch (Exception e)
        {
            if (OnUpdateError != null) await OnUpdateError.Invoke(e.Message);
        }
    }

    /// <summary>
    /// 查询电量
    /// </summary>
    public virtual async Task GetBatteryLevel()
    {
        try
        {
            await module!.InvokeVoidAsync("getBatteryLevel", InstanceBatteryLevel, BatteryLevelElement);
        }
        catch (Exception e)
        {
            if (OnUpdateError != null) await OnUpdateError.Invoke(e.Message);
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        InstanceBatteryLevel?.Dispose();
        if (module is not null)
        {
            await module.DisposeAsync();
        }
    }

    /// <summary>
    /// 设备名称回调方法
    /// </summary>
    /// <param name="devicename"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task UpdateDevicename(string devicename)
    {
        Device!.Name = devicename;
        if (OnUpdateStatus != null) await OnUpdateStatus.Invoke(Device);
    }

    /// <summary>
    /// 设备电量%回调方法
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task UpdateValue(decimal value)
    {
        Device!.Value = value;
        if (OnUpdateValue != null) await OnUpdateValue.Invoke(value);
    }

    /// <summary>
    /// 状态更新回调方法
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task UpdateStatus(string status)
    {
        Device!.Status = status;
        if (OnUpdateStatus != null) await OnUpdateStatus.Invoke(Device);
    }

    /// <summary>
    /// 错误更新回调方法
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    [JSInvokable]
    public async Task UpdateError(string status)
    {
        if (OnUpdateError != null) await OnUpdateError.Invoke(status);
    }

}
