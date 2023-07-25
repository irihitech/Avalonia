﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Platform;
using Avalonia.Win32.Interop.Automation;

namespace Avalonia.Win32.Automation
{
    [RequiresUnreferencedCode("Requires .NET COM interop")]
    internal class RootAutomationNode : AutomationNode,
        IRawElementProviderFragmentRoot,
        IRawElementProviderAdviseEvents
    {
        private int _raiseFocusChanged;

        public RootAutomationNode(AutomationPeer peer)
            : base(peer)
        {
            Peer = base.Peer.GetProvider<IRootProvider>() ?? throw new AvaloniaInternalException(
                "Attempt to create RootAutomationNode from peer which does not implement IRootProvider.");
            Peer.FocusChanged += FocusChanged;
        }

        public override IRawElementProviderFragmentRoot? FragmentRoot => this;
        public new IRootProvider Peer { get; }
        public IWindowBaseImpl? WindowImpl => Peer.PlatformImpl as IWindowBaseImpl;

        public IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
        {
            if (WindowImpl is null)
                return null;

            var p = WindowImpl.PointToClient(new PixelPoint((int)x, (int)y));
            var found = InvokeSync(() => GetPeerFromPoint(p));
            var result = GetOrCreate(found) as IRawElementProviderFragment;
            return result;
        }

        public IRawElementProviderFragment? GetFocus()
        {
            var focus = InvokeSync(() => Peer.GetFocus());
            return GetOrCreate(focus);
        }

        void IRawElementProviderAdviseEvents.AdviseEventAdded(int eventId, int[] properties)
        {
            switch ((UiaEventId)eventId)
            {
                case UiaEventId.AutomationFocusChanged:
                    ++_raiseFocusChanged;
                    break;
            }
        }

        void IRawElementProviderAdviseEvents.AdviseEventRemoved(int eventId, int[] properties)
        {
            switch ((UiaEventId)eventId)
            {
                case UiaEventId.AutomationFocusChanged:
                    --_raiseFocusChanged;
                    break;
            }
        }

        protected void RaiseFocusChanged(AutomationNode? focused)
        {
            if (_raiseFocusChanged > 0)
            {
                UiaCoreProviderApi.UiaRaiseAutomationEvent(
                    focused,
                    (int)UiaEventId.AutomationFocusChanged);
            }
        }

        public void FocusChanged(object? sender, EventArgs e)
        {
            RaiseFocusChanged(GetOrCreate(Peer.GetFocus()));
        }

        public Rect ToScreen(Rect rect)
        {
            if (WindowImpl is null)
                return default;
            return new PixelRect(
                WindowImpl.PointToScreen(rect.TopLeft),
                WindowImpl.PointToScreen(rect.BottomRight))
                    .ToRect(1);
        }

        public override IRawElementProviderSimple? HostRawElementProvider
        {
            get
            {
                var handle = WindowImpl?.Handle.Handle ?? IntPtr.Zero;
                if (handle == IntPtr.Zero)
                    return null;
                var hr = UiaCoreProviderApi.UiaHostProviderFromHwnd(handle, out var result);
                Marshal.ThrowExceptionForHR(hr);
                return result;
            }
        }

        private AutomationPeer? GetPeerFromPoint(Point p)
        {
            var hit = Peer.GetPeerFromPoint(p);

            while (hit != Peer && hit?.GetProvider<IRootProvider>() is { } embeddedRoot)
                hit = embeddedRoot.GetPeerFromPoint(p);
                
            return hit;
        }
    }
}
