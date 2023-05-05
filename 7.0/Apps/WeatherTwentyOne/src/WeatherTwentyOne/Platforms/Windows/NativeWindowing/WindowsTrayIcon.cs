using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hardcodet.Wpf.TaskbarNotification.Interop
{
	public class WindowsTrayIcon
	{
        private readonly object lockObject = new object();


        private NotifyIconData iconData;

        /// <summary>
        /// Receives messages from the taskbar icon.
        /// </summary>
        private readonly WindowMessageSink messageSink;

        public Action LeftClick { get; set; }
        public Action RightClick { get; set; }


        public bool IsTaskbarIconCreated { get; private set; }

        public WindowsTrayIcon(string iconFile)
		{
            messageSink = new WindowMessageSink();

            // init icon data structure
            iconData = NotifyIconData.CreateDefault(messageSink.MessageWindowHandle, iconFile);

            //IntPtr hIcon = PInvoke.User32.LoadImage(IntPtr.Zero, "dotnetbot.ico",
            //    PInvoke.User32.ImageType.IMAGE_ICON, 16, 16, PInvoke.User32.LoadImageFlags.LR_LOADFROMFILE);

            //PInvoke.User32.SendMessage(iconData.WindowHandle, PInvoke.User32.WindowMessage.WM_SETICON, (IntPtr)0, hIcon);

            // create the taskbar icon
            CreateTaskbarIcon();

			// register event listeners
			messageSink.MouseEventReceived += MessageSink_MouseEventReceived;
			messageSink.TaskbarCreated += MessageSink_TaskbarCreated;
            //messageSink.ChangeToolTipStateRequest += OnToolTipChange;
        }

		private void MessageSink_TaskbarCreated()
		{
            RemoveTaskbarIcon();
            CreateTaskbarIcon();
        }

		private void MessageSink_MouseEventReceived(MouseEvent obj)
		{
			if (obj == MouseEvent.IconLeftMouseUp)
			{
                LeftClick?.Invoke();
			} else if (obj == MouseEvent.IconRightMouseUp)
			{
                RightClick?.Invoke();
			}
		}

		private void CreateTaskbarIcon()
        {
            lock (lockObject)
            {
                if (IsTaskbarIconCreated)
                {
                    return;
                }

                const IconDataMembers members = IconDataMembers.Message
                                                | IconDataMembers.Icon
                                                | IconDataMembers.Tip;

                //write initial configuration
                var status = WriteIconData(ref iconData, NotifyCommand.Add, members);
                if (!status)
                {
                    // couldn't create the icon - we can assume this is because explorer is not running (yet!)
                    // -> try a bit later again rather than throwing an exception. Typically, if the windows
                    // shell is being loaded later, this method is being re-invoked from OnTaskbarCreated
                    // (we could also retry after a delay, but that's currently YAGNI)
                    return;
                }

                //set to most recent version
                SetVersion();
                //messageSink.Version = (NotifyIconVersion)iconData.VersionOrTimeout;

                IsTaskbarIconCreated = true;
            }
        }

        private void RemoveTaskbarIcon()
        {
            lock (lockObject)
            {
                // make sure we didn't schedule a creation

                if (!IsTaskbarIconCreated)
                {
                    return;
                }

                WriteIconData(ref iconData, NotifyCommand.Delete, IconDataMembers.Message);
                IsTaskbarIconCreated = false;
            }
        }


        private void SetVersion()
        {
            iconData.VersionOrTimeout = (uint)0x4;
            bool status = WinApi.Shell_NotifyIcon(NotifyCommand.SetVersion, ref iconData);

            if (!status)
            {
                Debug.Fail("Could not set version");
            }
        }

        public static readonly object SyncRoot = new object();


        /// <summary>
        /// Updates the taskbar icons with data provided by a given
        /// <see cref="NotifyIconData"/> instance.
        /// </summary>
        /// <param name="data">Configuration settings for the NotifyIcon.</param>
        /// <param name="command">Operation on the icon (e.g. delete the icon).</param>
        /// <returns>True if the data was successfully written.</returns>
        /// <remarks>See Shell_NotifyIcon documentation on MSDN for details.</remarks>
        public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command)
        {
            return WriteIconData(ref data, command, data.ValidMembers);
        }


        /// <summary>
        /// Updates the taskbar icons with data provided by a given
        /// <see cref="NotifyIconData"/> instance.
        /// </summary>
        /// <param name="data">Configuration settings for the NotifyIcon.</param>
        /// <param name="command">Operation on the icon (e.g. delete the icon).</param>
        /// <param name="flags">Defines which members of the <paramref name="data"/>
        /// structure are set.</param>
        /// <returns>True if the data was successfully written.</returns>
        /// <remarks>See Shell_NotifyIcon documentation on MSDN for details.</remarks>
        public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command, IconDataMembers flags)
        {

            data.ValidMembers = flags;
            lock (SyncRoot)
            {
                return WinApi.Shell_NotifyIcon(command, ref data);
            }
        }
    }
}
