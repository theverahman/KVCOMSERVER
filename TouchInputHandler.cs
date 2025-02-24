using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public enum TouchAction
{
    Down,
    Move,
    Up
}

public class TouchEventArgs : EventArgs
{
    public TouchAction Action { get; }
    public List<TouchPoint> TouchPoints { get; }

    public TouchEventArgs(TouchAction action, List<TouchPoint> touchPoints)
    {
        Action = action;
        TouchPoints = touchPoints;
    }

    public List<TouchPoint> GetTouchPoints(Control control)
    {
        return TouchPoints;
    }
}

public class TouchPoint
{
    public Point Position { get; }

    public TouchPoint(Point position)
    {
        Position = position;
    }
}

public class TouchEventHandler
{
    private Point lastTouchPoint1;
    private Point lastTouchPoint2;
    private bool isZooming = false;

    public void HandleTouch(object sender, TouchEventArgs e)
    {
        Control control = sender as Control;
        if (control != null)
        {
            if (e.Action == TouchAction.Down)
            {
                if (e.GetTouchPoints(control).Count == 2)
                {
                    isZooming = true;
                    lastTouchPoint1 = e.GetTouchPoints(control)[0].Position;
                    lastTouchPoint2 = e.GetTouchPoints(control)[1].Position;
                }
            }
            else if (e.Action == TouchAction.Move && isZooming)
            {
                if (e.GetTouchPoints(control).Count == 2)
                {
                    Point newTouchPoint1 = e.GetTouchPoints(control)[0].Position;
                    Point newTouchPoint2 = e.GetTouchPoints(control)[1].Position;

                    double oldDistance = Distance(lastTouchPoint1, lastTouchPoint2);
                    double newDistance = Distance(newTouchPoint1, newTouchPoint2);

                    int delta = (int)((newDistance - oldDistance) * 120); // Scale factor for zooming

                    // Raise the MouseWheel event
                    var mouseEventArgs = new MouseEventArgs(MouseButtons.None, 0, newTouchPoint1.X, newTouchPoint1.Y, delta);
                    MethodInfo onMouseWheelMethod = control.GetType().GetMethod("OnMouseWheel", BindingFlags.NonPublic | BindingFlags.Instance);
                    onMouseWheelMethod?.Invoke(control, new object[] { mouseEventArgs });

                    lastTouchPoint1 = newTouchPoint1;
                    lastTouchPoint2 = newTouchPoint2;
                }
            }
            else if (e.Action == TouchAction.Up)
            {
                isZooming = false;
            }
        }
    }

    private double Distance(Point p1, Point p2)
    {
        int dx = p1.X - p2.X;
        int dy = p1.Y - p2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}

public class NativeTouchHandler : NativeWindow
{
    private const int WM_TOUCH = 0x0240;
    private const int TOUCHEVENTF_DOWN = 0x0001;
    private const int TOUCHEVENTF_UP = 0x0002;
    private const int TOUCHEVENTF_MOVE = 0x0004;

    private const int TOUCHINPUTMASKF_CONTACTAREA = 0x0004;
    private const int TOUCHINPUTMASKF_EXTRAINFO = 0x0002;
    private const int TOUCHINPUTMASKF_TIMEFROMSYSTEM = 0x0001;

    [StructLayout(LayoutKind.Sequential)]
    private struct TOUCHINPUT
    {
        public int x;
        public int y;
        public IntPtr hSource;
        public int dwID;
        public int dwFlags;
        public int dwMask;
        public int dwTime;
        public IntPtr dwExtraInfo;
        public int cxContact;
        public int cyContact;
    }

    [DllImport("user32")]
    private static extern bool RegisterTouchWindow(IntPtr hWnd, uint ulFlags);

    [DllImport("user32")]
    private static extern bool UnregisterTouchWindow(IntPtr hWnd);

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [In, Out] TOUCHINPUT[] pInputs, int cbSize);

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern void CloseTouchInputHandle(IntPtr lParam);

    private Control control;
    private TouchEventHandler touchEventHandler;

    public NativeTouchHandler(Control control, TouchEventHandler touchEventHandler)
    {
        this.control = control;
        this.touchEventHandler = touchEventHandler;
        AssignHandle(control.Handle);
        RegisterTouchWindow(control.Handle, 0);
    }

    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case WM_TOUCH:
                DecodeTouch(ref m);
                break;
        }
        base.WndProc(ref m);
    }

    private void DecodeTouch(ref Message m)
    {
        int inputCount = m.WParam.ToInt32();
        TOUCHINPUT[] inputs = new TOUCHINPUT[inputCount];
        if (GetTouchInputInfo(m.LParam, inputCount, inputs, Marshal.SizeOf(typeof(TOUCHINPUT))))
        {
            List<TouchPoint> touchPoints = new List<TouchPoint>();
            foreach (var input in inputs)
            {
                Point pt = control.PointToClient(new Point(input.x / 100, input.y / 100));
                touchPoints.Add(new TouchPoint(pt));
            }

            TouchAction action = TouchAction.Move;
            if (inputs[0].dwFlags == TOUCHEVENTF_DOWN)
                action = TouchAction.Down;
            else if (inputs[0].dwFlags == TOUCHEVENTF_UP)
                action = TouchAction.Up;

            var touchEventArgs = new TouchEventArgs(action, touchPoints);
            touchEventHandler.HandleTouch(control, touchEventArgs);
            CloseTouchInputHandle(m.LParam);
        }
    }

    public void Dispose()
    {
        UnregisterTouchWindow(control.Handle);
        ReleaseHandle();
    }
}