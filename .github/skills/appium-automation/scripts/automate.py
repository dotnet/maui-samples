#!/usr/bin/env python3
"""
Cross-Platform Appium Agent for Mobile App Automation
Supports iOS Simulator, Android Emulator, and Mac Catalyst.

Usage:
    from appium_agent import AppiumAgent
    
    with AppiumAgent(platform="ios", app_id="com.example.myapp") as agent:
        agent.tap("MyButton")
        agent.type_text("EmailField", "user@example.com")
        text = agent.get_text("ResultLabel")

CLI:
    python appium_agent.py --platform ios --app-id com.example.myapp --tap MyButton
"""

import json
import time
import base64
import subprocess
import sys
import os
import socket
import signal
import hashlib
from pathlib import Path
from typing import Optional, Dict, Any, List, Tuple
from dataclasses import dataclass
from enum import Enum
from contextlib import contextmanager

# Session cache directory
SESSION_CACHE_DIR = Path("/tmp/appium-sessions")

# Appium imports
from appium import webdriver
from appium.options.ios import XCUITestOptions
from appium.options.android import UiAutomator2Options
from appium.options.mac import Mac2Options
from appium.webdriver.common.appiumby import AppiumBy
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import NoSuchElementException, TimeoutException
from selenium.webdriver.common.action_chains import ActionChains
from selenium.webdriver.common.actions import interaction
from selenium.webdriver.common.actions.action_builder import ActionBuilder
from selenium.webdriver.common.actions.pointer_input import PointerInput


class Platform(Enum):
    IOS = "ios"
    ANDROID = "android"
    MACCATALYST = "maccatalyst"


@dataclass
class DeviceInfo:
    """Device/simulator/emulator information."""
    udid: str
    name: str
    platform: Platform
    version: Optional[str] = None
    state: str = "unknown"


class AppiumAgent:
    """Cross-platform agent for automating mobile apps via Appium."""
    
    def __init__(
        self,
        platform: str,
        app_id: str,
        appium_url: str = "http://127.0.0.1:4723",
        udid: Optional[str] = None,
        app_path: Optional[str] = None,
        auto_grant_permissions: bool = True,
        no_reset: bool = True,
        reuse_session: bool = False,
        keep_session: bool = False,
    ):
        """
        Initialize the agent.
        
        Args:
            platform: Target platform ('ios', 'android', 'maccatalyst')
            app_id: App identifier (bundle ID for iOS/Catalyst, package name for Android)
            appium_url: Appium server URL
            udid: Device UDID (auto-detects if None)
            app_path: Path to .app/.apk to install (optional)
            auto_grant_permissions: Auto-grant Android permissions (default: True)
            no_reset: Don't reset app state between sessions (default: True)
            reuse_session: Try to reuse a cached session (default: False)
            keep_session: Keep session alive after disconnect (default: False)
        """
        self.platform = Platform(platform.lower())
        self.app_id = app_id
        self.appium_url = appium_url
        self.app_path = app_path
        self.auto_grant_permissions = auto_grant_permissions
        self.no_reset = no_reset
        self.reuse_session = reuse_session
        self.keep_session = keep_session
        self.driver: Optional[webdriver.Remote] = None
        self._session_reused = False
        
        # Auto-detect device if not specified
        self.udid = udid or self._detect_device()
        self._setup_options()
    
    def _detect_device(self) -> str:
        """Auto-detect available device/simulator/emulator."""
        if self.platform == Platform.IOS:
            return self._get_booted_ios_simulator()
        elif self.platform == Platform.ANDROID:
            return self._get_android_device()
        elif self.platform == Platform.MACCATALYST:
            return "mac"  # Mac Catalyst runs on the Mac itself
        raise RuntimeError(f"Unknown platform: {self.platform}")
    
    def _get_booted_ios_simulator(self) -> str:
        """Get UDID of first booted iOS simulator."""
        result = subprocess.run(
            ["xcrun", "simctl", "list", "devices", "booted", "-j"],
            capture_output=True, text=True
        )
        data = json.loads(result.stdout)
        for runtime, devices in data.get("devices", {}).items():
            if "iOS" in runtime:
                for device in devices:
                    if device.get("state") == "Booted":
                        return device["udid"]
        raise RuntimeError(
            "No booted iOS simulator found.\n"
            "Boot one with: xcrun simctl boot 'iPhone 16 Pro'"
        )
    
    def _get_android_device(self) -> str:
        """Get first connected Android device/emulator."""
        result = subprocess.run(
            ["adb", "devices"],
            capture_output=True, text=True
        )
        lines = result.stdout.strip().split('\n')[1:]  # Skip header
        for line in lines:
            if '\tdevice' in line:
                return line.split('\t')[0]
        raise RuntimeError(
            "No Android device/emulator found.\n"
            "Start emulator with: emulator -avd <name>\n"
            "Or connect a device with USB debugging enabled."
        )
    
    def _setup_options(self):
        """Configure Appium options based on platform."""
        if self.platform == Platform.IOS:
            self.options = XCUITestOptions()
            self.options.platform_name = "iOS"
            self.options.device_name = "iPhone"
            self.options.udid = self.udid
            self.options.automation_name = "XCUITest"
            self.options.bundle_id = self.app_id
            self.options.no_reset = self.no_reset
            if self.app_path:
                self.options.app = self.app_path
                
        elif self.platform == Platform.ANDROID:
            self.options = UiAutomator2Options()
            self.options.platform_name = "Android"
            self.options.udid = self.udid
            self.options.automation_name = "UiAutomator2"
            self.options.app_package = self.app_id
            self.options.no_reset = self.no_reset
            self.options.auto_grant_permissions = self.auto_grant_permissions
            if self.app_path:
                self.options.app = self.app_path
            # Required for .NET MAUI apps with Fast Deployment
            self.options.set_capability("appium:noReset", True)
                
        elif self.platform == Platform.MACCATALYST:
            self.options = Mac2Options()
            self.options.platform_name = "Mac"
            self.options.automation_name = "Mac2"
            self.options.bundle_id = self.app_id
            self.options.no_reset = self.no_reset
            if self.app_path:
                self.options.app = self.app_path
    
    def _get_session_cache_key(self) -> str:
        """Generate a unique cache key for this session configuration."""
        key_parts = f"{self.platform.value}:{self.app_id}:{self.udid}:{self.appium_url}"
        return hashlib.md5(key_parts.encode()).hexdigest()[:12]
    
    def _get_session_cache_file(self) -> Path:
        """Get the session cache file path."""
        SESSION_CACHE_DIR.mkdir(parents=True, exist_ok=True)
        return SESSION_CACHE_DIR / f"session_{self._get_session_cache_key()}.json"
    
    def _save_session(self):
        """Save current session to cache."""
        if self.driver and self.driver.session_id:
            cache_data = {
                "session_id": self.driver.session_id,
                "platform": self.platform.value,
                "app_id": self.app_id,
                "appium_url": self.appium_url,
                "timestamp": time.time(),
            }
            cache_file = self._get_session_cache_file()
            cache_file.write_text(json.dumps(cache_data))
    
    def _load_cached_session(self) -> Optional[Dict]:
        """Load cached session if it exists and is recent."""
        cache_file = self._get_session_cache_file()
        if not cache_file.exists():
            return None
        try:
            data = json.loads(cache_file.read_text())
            # Session expires after 5 minutes of inactivity
            if time.time() - data.get("timestamp", 0) > 300:
                cache_file.unlink(missing_ok=True)
                return None
            return data
        except:
            return None
    
    def _clear_session_cache(self):
        """Clear the session cache file."""
        cache_file = self._get_session_cache_file()
        cache_file.unlink(missing_ok=True)
    
    def _try_reuse_session(self) -> bool:
        """Try to reuse a cached session. Returns True if successful."""
        cached = self._load_cached_session()
        if not cached:
            return False
        
        try:
            import urllib.request
            from appium.webdriver.errorhandler import MobileErrorHandler
            from selenium.webdriver.remote.remote_connection import RemoteConnection
            from selenium.webdriver.remote.file_detector import LocalFileDetector
            from selenium.webdriver.remote.locator_converter import LocatorConverter
            from selenium.webdriver.remote.mobile import Mobile
            from selenium.webdriver.remote.shadowroot import ShadowRoot
            from selenium.webdriver.remote.switch_to import SwitchTo
            
            # Test if session is still valid
            req = urllib.request.Request(
                f"{self.appium_url}/session/{cached['session_id']}"
            )
            try:
                with urllib.request.urlopen(req, timeout=5) as resp:
                    if resp.status != 200:
                        self._clear_session_cache()
                        return False
                    caps_data = json.loads(resp.read().decode())
            except:
                self._clear_session_cache()
                return False
            
            # Create driver attached to existing session (no new session)
            from selenium.webdriver.remote.client_config import ClientConfig
            from appium.webdriver.appium_connection import AppiumConnection
            from appium.webdriver.mobilecommand import MobileCommand
            config = ClientConfig(remote_server_addr=self.appium_url)
            executor = AppiumConnection(client_config=config)
            
            # Add Appium-specific commands that are normally added in webdriver.__init__
            executor.add_command(MobileCommand.CLEAR, 'POST', '/session/$sessionId/element/$id/clear')
            
            # Construct driver with all required attributes
            self.driver = object.__new__(webdriver.Remote)
            self.driver.command_executor = executor
            self.driver.session_id = cached["session_id"]
            self.driver._is_remote = True
            self.driver.caps = caps_data.get("value", {}).get("capabilities", {})
            self.driver.pinned_scripts = {}
            self.driver.error_handler = MobileErrorHandler()
            self.driver._file_detector = LocalFileDetector()
            self.driver.locator_converter = LocatorConverter()
            self.driver._switch_to = SwitchTo(self.driver)
            self.driver._mobile = Mobile(self.driver)
            self.driver._bidi_connection = None
            self.driver._fedcm = None
            self.driver._script = None
            self.driver._absent_extensions = set()  # Required for activate_app/terminate_app
            
            self._session_reused = True
            self._save_session()  # Update timestamp
            return True
                
        except Exception as e:
            self._clear_session_cache()
            return False
    
    def connect(self) -> "AppiumAgent":
        """Connect to Appium and attach to the app."""
        if self.reuse_session:
            if self._try_reuse_session():
                self.driver.implicitly_wait(5)
                return self
        
        self.driver = webdriver.Remote(self.appium_url, options=self.options)
        self.driver.implicitly_wait(5)
        
        if self.keep_session:
            self._save_session()
        
        return self
    
    def disconnect(self):
        """Disconnect from Appium (keeps app running)."""
        if self.driver:
            if self.keep_session:
                # Don't quit, just save and detach
                self._save_session()
                self.driver = None
            else:
                self.driver.quit()
                self._clear_session_cache()
                self.driver = None
    
    @classmethod
    def end_all_sessions(cls, appium_url: str = "http://127.0.0.1:4723"):
        """End all cached sessions."""
        import urllib.request
        if SESSION_CACHE_DIR.exists():
            for cache_file in SESSION_CACHE_DIR.glob("session_*.json"):
                try:
                    data = json.loads(cache_file.read_text())
                    session_id = data.get("session_id")
                    if session_id:
                        req = urllib.request.Request(
                            f"{appium_url}/session/{session_id}",
                            method="DELETE"
                        )
                        urllib.request.urlopen(req, timeout=5)
                except:
                    pass
                cache_file.unlink(missing_ok=True)
    
    def __enter__(self):
        return self.connect()
    
    def __exit__(self, *args):
        self.disconnect()
    
    # ==================== Element Finding ====================
    
    def find_element(self, identifier: str, timeout: float = 10):
        """
        Find element by accessibility ID, automation ID, or name.
        
        Args:
            identifier: Element identifier (AutomationId in MAUI)
            timeout: Max seconds to wait (total, not per-strategy)
            
        Returns:
            WebElement or None
        """
        import time
        start_time = time.time()
        
        strategies = [
            (AppiumBy.ACCESSIBILITY_ID, identifier),
            (AppiumBy.ID, identifier),
            (AppiumBy.NAME, identifier),
        ]
        
        # Platform-specific strategies
        if self.platform == Platform.ANDROID:
            strategies.append((AppiumBy.ID, f"{self.app_id}:id/{identifier}"))
        
        # Calculate per-strategy timeout
        per_strategy_timeout = max(0.5, timeout / len(strategies))
        
        for by, value in strategies:
            # Check if we've exceeded total timeout
            elapsed = time.time() - start_time
            if elapsed >= timeout:
                break
            
            # Use remaining time or per-strategy timeout, whichever is smaller
            remaining = timeout - elapsed
            strategy_timeout = min(per_strategy_timeout, remaining)
            
            try:
                return WebDriverWait(self.driver, strategy_timeout).until(
                    EC.presence_of_element_located((by, value))
                )
            except (NoSuchElementException, TimeoutException):
                continue
        return None
    
    def find_elements(self, identifier: str) -> List:
        """Find all elements matching identifier."""
        elements = []
        for by in [AppiumBy.ACCESSIBILITY_ID, AppiumBy.ID, AppiumBy.NAME]:
            try:
                found = self.driver.find_elements(by, identifier)
                elements.extend(found)
            except:
                pass
        return elements
    
    # ==================== Gesture Helper ====================
    
    def _perform_gesture(
        self,
        start_x: int, start_y: int,
        end_x: int, end_y: int,
        duration_ms: int = 300
    ):
        """
        Perform a pointer gesture from start to end coordinates.
        
        Args:
            start_x, start_y: Starting position
            end_x, end_y: Ending position
            duration_ms: Duration of gesture in milliseconds
        """
        actions = ActionChains(self.driver)
        finger = actions.w3c_actions.add_pointer_input('touch', 'finger')
        finger.create_pointer_move(x=start_x, y=start_y)
        finger.create_pointer_down(button=0)
        finger.create_pointer_move(x=end_x, y=end_y, duration=duration_ms)
        finger.create_pointer_up(button=0)
        actions.perform()
    
    # ==================== Core Actions ====================
    
    def tap(self, identifier: str, timeout: float = 10) -> bool:
        """
        Tap an element by identifier.
        
        Args:
            identifier: Element's accessibility/automation ID
            timeout: Max seconds to wait for element
            
        Returns:
            True if tapped successfully
        """
        element = self.find_element(identifier, timeout)
        if element:
            element.click()
            return True
        return False
    
    def tap_text(self, text: str, timeout: float = 10) -> bool:
        """
        Tap element containing specific text.
        
        Args:
            text: Text to find
            timeout: Max seconds to wait
            
        Returns:
            True if tapped successfully
        """
        xpath_patterns = [
            f"//*[@text='{text}']",
            f"//*[@label='{text}']",
            f"//*[@name='{text}']",
            f"//*[contains(@text, '{text}')]",
            f"//*[contains(@label, '{text}')]",
        ]
        
        for xpath in xpath_patterns:
            try:
                element = WebDriverWait(self.driver, timeout / len(xpath_patterns)).until(
                    EC.presence_of_element_located((AppiumBy.XPATH, xpath))
                )
                element.click()
                return True
            except (NoSuchElementException, TimeoutException):
                continue
        return False
    
    def tap_button(self, text: str, timeout: float = 10) -> bool:
        """
        Tap a button by its visible text/label.
        
        Args:
            text: Button's visible text
            timeout: Max seconds to wait
            
        Returns:
            True if tapped successfully
        """
        if self.platform == Platform.IOS or self.platform == Platform.MACCATALYST:
            xpath = f"//XCUIElementTypeButton[@name='{text}' or @label='{text}']"
        else:  # Android
            xpath = f"//android.widget.Button[@text='{text}' or @content-desc='{text}']"
        
        try:
            element = WebDriverWait(self.driver, timeout).until(
                EC.presence_of_element_located((AppiumBy.XPATH, xpath))
            )
            element.click()
            return True
        except (NoSuchElementException, TimeoutException):
            # Fallback to generic tap
            return self.tap(text, timeout=2) or self.tap_text(text, timeout=2)
    
    def double_tap(self, identifier: str, timeout: float = 10) -> bool:
        """Double-tap an element."""
        element = self.find_element(identifier, timeout)
        if element:
            actions = ActionChains(self.driver)
            actions.double_click(element).perform()
            return True
        return False
    
    def long_press(self, identifier: str, duration: float = 2.0, timeout: float = 10) -> bool:
        """
        Long press (press and hold) an element.
        
        Args:
            identifier: Element identifier
            duration: Hold duration in seconds
            timeout: Max seconds to wait for element
        """
        element = self.find_element(identifier, timeout)
        if element:
            actions = ActionBuilder(self.driver)
            finger = PointerInput(interaction.POINTER_TOUCH, "finger")
            actions.add_pointer_input(finger)
            
            location = element.location
            size = element.size
            center_x = location['x'] + size['width'] // 2
            center_y = location['y'] + size['height'] // 2
            
            finger.create_pointer_move(x=center_x, y=center_y)
            finger.create_pointer_down(button=0)
            finger.create_pause(duration)
            finger.create_pointer_up(button=0)
            actions.perform()
            return True
        return False
    
    def type_text(self, identifier: str, text: str, clear: bool = True) -> bool:
        """
        Type text into an input field.
        
        Args:
            identifier: Element's identifier
            text: Text to type
            clear: Clear existing text first
            
        Returns:
            True if successful
        """
        element = self.find_element(identifier)
        if element:
            if clear:
                element.clear()
            element.send_keys(text)
            return True
        return False
    
    def get_text(self, identifier: str) -> Optional[str]:
        """
        Get text content of an element.
        
        Args:
            identifier: Element identifier
            
        Returns:
            Element's text, or None if not found
        """
        element = self.find_element(identifier, timeout=5)
        if element:
            # Platform-specific attribute order
            if self.platform in ['ios', 'maccatalyst']:
                attrs = ['value', 'label', 'name']
            else:  # android
                attrs = ['text', 'content-desc', 'value']
            for attr in attrs:
                try:
                    text = element.get_attribute(attr)
                    if text:
                        return text
                except:
                    continue
            return element.text
        return None
    
    def get_attribute(self, identifier: str, attribute: str) -> Optional[str]:
        """Get specific attribute of an element."""
        element = self.find_element(identifier, timeout=5)
        if element:
            return element.get_attribute(attribute)
        return None
    
    def exists(self, identifier: str, timeout: float = 2) -> bool:
        """Check if an element exists."""
        return self.find_element(identifier, timeout=timeout) is not None
    
    def expect(self, identifier: str, expected: str) -> Tuple[bool, Optional[str]]:
        """
        Assert element contains expected text.
        
        Args:
            identifier: Element identifier
            expected: Expected text (substring match)
            
        Returns:
            Tuple of (passed, actual_value)
        """
        actual = self.get_text(identifier)
        if actual is None:
            return False, None
        passed = expected in actual
        return passed, actual
    
    def clear(self, identifier: str) -> bool:
        """Clear text from an input field."""
        element = self.find_element(identifier)
        if element:
            element.clear()
            return True
        return False
    
    def is_enabled(self, identifier: str) -> Optional[bool]:
        """Check if an element is enabled/interactable."""
        element = self.find_element(identifier)
        if element:
            return element.is_enabled()
        return None
    
    def is_visible(self, identifier: str) -> bool:
        """Check if element is displayed/visible on screen."""
        element = self.find_element(identifier, timeout=2)
        if element:
            try:
                # Check element has non-zero size and is on-screen
                rect = element.rect
                if rect['width'] <= 0 or rect['height'] <= 0:
                    return False
                
                # Check if element center is within viewport
                window_size = self.driver.get_window_size()
                center_x = rect['x'] + rect['width'] / 2
                center_y = rect['y'] + rect['height'] / 2
                
                if center_x < 0 or center_x > window_size['width']:
                    return False
                if center_y < 0 or center_y > window_size['height']:
                    return False
                
                return True
            except:
                return False
        return False
    
    def scroll(self, direction: str = 'down', amount: float = 0.5) -> bool:
        """
        Scroll the screen in a direction.
        
        Args:
            direction: 'up', 'down', 'left', 'right'
            amount: Scroll distance as fraction of screen (0.0-1.0)
        """
        try:
            size = self.driver.get_window_size()
            center_x = size['width'] // 2
            center_y = size['height'] // 2
            
            offset = int(min(size['width'], size['height']) * amount)
            
            if direction == 'up':
                start_y, end_y = center_y + offset // 2, center_y - offset // 2
                start_x = end_x = center_x
            elif direction == 'down':
                start_y, end_y = center_y - offset // 2, center_y + offset // 2
                start_x = end_x = center_x
            elif direction == 'left':
                start_x, end_x = center_x + offset // 2, center_x - offset // 2
                start_y = end_y = center_y
            elif direction == 'right':
                start_x, end_x = center_x - offset // 2, center_x + offset // 2
                start_y = end_y = center_y
            else:
                return False
            
            self._perform_gesture(start_x, start_y, end_x, end_y, duration_ms=300)
            return True
        except Exception as e:
            print(f"scroll error: {e}")
            return False
    
    def find_like(self, partial_id: str, timeout: float = 5) -> Optional[Any]:
        """
        Find element with partial ID match (fuzzy matching).
        
        Args:
            partial_id: Partial identifier to match
            
        Returns:
            First matching element, or None
        """
        try:
            # Parse page source for partial matches
            source = self.driver.page_source
            import xml.etree.ElementTree as ET
            root = ET.fromstring(source)
            
            partial_lower = partial_id.lower()
            
            for elem in root.iter():
                # Check various ID attributes
                for attr in ['name', 'identifier', 'resource-id', 'accessibility-id', 'content-desc']:
                    value = elem.get(attr, '')
                    if value and partial_lower in value.lower():
                        # Found a match, now find via Appium
                        return self.find_element(value.split('/')[-1], timeout=timeout)
            
            # Also try xpath contains
            if self.platform in [Platform.IOS, Platform.MACCATALYST]:
                xpath = f"//*[contains(@name, '{partial_id}')]"
            else:
                xpath = f"//*[contains(@resource-id, '{partial_id}') or contains(@content-desc, '{partial_id}')]"
            
            return WebDriverWait(self.driver, timeout).until(
                EC.presence_of_element_located((AppiumBy.XPATH, xpath))
            )
        except:
            return None
    
    def tap_like(self, partial_id: str, timeout: float = 10) -> bool:
        """Tap element with partial ID match."""
        element = self.find_like(partial_id, timeout)
        if element:
            element.click()
            return True
        return False
    
    def dismiss_keyboard(self) -> bool:
        """Dismiss the on-screen keyboard."""
        try:
            if self.platform == Platform.ANDROID:
                self.driver.hide_keyboard()
            else:  # iOS/Mac Catalyst
                # Multiple strategies to hide keyboard on iOS
                dismissed = False
                
                # Strategy 1: Send return key to active element (most reliable for MAUI)
                try:
                    active = self.driver.switch_to.active_element
                    if active:
                        active.send_keys('\n')
                        dismissed = True
                except:
                    pass
                
                # Strategy 2: Use mobile: hideKeyboard command
                if not dismissed:
                    try:
                        self.driver.execute_script('mobile: hideKeyboard', {'keys': ['return']})
                        dismissed = True
                    except:
                        pass
                
                # Strategy 3: Tap Done button if visible
                if not dismissed:
                    try:
                        done_btn = self.driver.find_element(AppiumBy.ACCESSIBILITY_ID, 'Done')
                        done_btn.click()
                    except:
                        pass
            return True
        except:
            return False
    
    def press_key(self, key: str) -> bool:
        """
        Press a keyboard key.
        
        Args:
            key: Key name (Enter, Tab, Escape, Backspace, Delete)
        """
        try:
            key_map = {
                'enter': '\n',
                'return': '\n',
                'tab': '\t',
                'backspace': '\b',
            }
            key_lower = key.lower()
            if key_lower in key_map:
                from selenium.webdriver.common.keys import Keys
                # Send key to active element
                active = self.driver.switch_to.active_element
                if active:
                    active.send_keys(key_map[key_lower])
                    return True
            return False
        except:
            return False
    
    def accept_alert(self) -> bool:
        """Accept/confirm an alert dialog."""
        try:
            self.driver.switch_to.alert.accept()
            return True
        except:
            return False
    
    def dismiss_alert(self) -> bool:
        """Dismiss/cancel an alert dialog."""
        try:
            self.driver.switch_to.alert.dismiss()
            return True
        except:
            return False
    
    def get_alert_text(self) -> Optional[str]:
        """Get text from current alert dialog."""
        try:
            return self.driver.switch_to.alert.text
        except:
            return None
    
    # ==================== Waiting ====================
    
    def wait(self, seconds: float = 1):
        """Wait for a duration (seconds)."""
        time.sleep(seconds)
    
    def wait_for(self, identifier: str, timeout: float = 10) -> bool:
        """
        Wait for an element to appear.
        
        Args:
            identifier: Element identifier
            timeout: Max seconds to wait
            
        Returns:
            True if element appeared
        """
        return self.find_element(identifier, timeout=timeout) is not None
    
    def wait_for_text(self, identifier: str, expected: str, timeout: float = 10) -> bool:
        """
        Wait for element to have specific text.
        
        Args:
            identifier: Element identifier
            expected: Expected text content
            timeout: Max seconds to wait
        """
        end_time = time.time() + timeout
        while time.time() < end_time:
            text = self.get_text(identifier)
            if text and expected in text:
                return True
            time.sleep(0.5)
        return False
    
    def wait_until_gone(self, identifier: str, timeout: float = 10) -> bool:
        """Wait for element to disappear."""
        end_time = time.time() + timeout
        while time.time() < end_time:
            if not self.exists(identifier, timeout=0.5):
                return True
            time.sleep(0.5)
        return False
    
    # ==================== Gestures ====================
    
    def swipe(self, direction: str = "up", duration: int = 500):
        """
        Swipe in a direction.
        
        Args:
            direction: 'up', 'down', 'left', or 'right'
            duration: Swipe duration in milliseconds
        """
        size = self.driver.get_window_size()
        w, h = size['width'], size['height']
        coords = {
            'up': (w//2, int(h*0.7), w//2, int(h*0.3)),
            'down': (w//2, int(h*0.3), w//2, int(h*0.7)),
            'left': (int(w*0.8), h//2, int(w*0.2), h//2),
            'right': (int(w*0.2), h//2, int(w*0.8), h//2),
        }
        start_x, start_y, end_x, end_y = coords.get(direction.lower(), coords['up'])
        self.driver.swipe(start_x, start_y, end_x, end_y, duration)
    
    def scroll_to(self, identifier: str, direction: str = "down", max_swipes: int = 5) -> bool:
        """
        Scroll until element is visible.
        
        Args:
            identifier: Element to find
            direction: Scroll direction ('up' or 'down')
            max_swipes: Maximum scroll attempts
            
        Returns:
            True if element found
        """
        for _ in range(max_swipes):
            if self.exists(identifier, timeout=1):
                return True
            self.swipe(direction)
            time.sleep(0.3)
        return self.exists(identifier, timeout=1)
    
    def tap_coords(self, x: int, y: int):
        """Tap at specific screen coordinates."""
        self.driver.execute_script('mobile: tap', {'x': x, 'y': y})
    
    def set_slider(self, identifier: str, value: float) -> bool:
        """
        Set slider to a specific value.
        
        Args:
            identifier: Slider element identifier
            value: Value 0-1 (decimal) or 0-100 (percentage). Auto-detects format.
            
        Returns:
            True if successful
        """
        element = self.find_element(identifier)
        if element:
            # Auto-detect: if value > 1, treat as percentage (0-100)
            if value > 1:
                value = value / 100.0
            # Clamp to valid range
            value = max(0.0, min(1.0, value))
            element.send_keys(str(value))
            return True
        return False
    
    def drag(
        self,
        identifier: str,
        offset_x: int = 0,
        offset_y: int = 0,
        duration: float = 1.0
    ) -> bool:
        """
        Drag an element by offset (useful for sliders).
        
        Args:
            identifier: Element identifier
            offset_x: Horizontal offset in pixels (positive = right)
            offset_y: Vertical offset in pixels (positive = down)
            duration: Drag duration in seconds (default: 1.0)
            
        Returns:
            True if successful
        """
        element = self.find_element(identifier)
        if element:
            rect = element.rect
            
            # For sliders, start from current thumb position
            element_type = element.get_attribute('type') or ''
            if 'Slider' in element_type:
                # Get current value as percentage
                value_str = element.get_attribute('value') or '50%'
                try:
                    pct = float(value_str.replace('%', '')) / 100.0
                except:
                    pct = 0.5
                from_x = rect['x'] + rect['width'] * pct
            else:
                # Start from center of element
                from_x = rect['x'] + rect['width'] / 2
            
            from_y = rect['y'] + rect['height'] / 2
            to_x = from_x + offset_x
            to_y = from_y + offset_y
            
            self.driver.execute_script('mobile: dragFromToForDuration', {
                'fromX': from_x,
                'fromY': from_y,
                'toX': to_x,
                'toY': to_y,
                'duration': duration
            })
            return True
        return False
    
    def drag_and_drop(
        self,
        source_id: str,
        target_id: str,
        duration: float = 1.0
    ) -> bool:
        """
        Drag from source element to target element.
        
        Args:
            source_id: Source element identifier
            target_id: Target element identifier
            duration: Drag duration in seconds
        """
        source = self.find_element(source_id)
        target = self.find_element(target_id)
        
        if source and target:
            src_loc = source.location
            src_size = source.size
            tgt_loc = target.location
            tgt_size = target.size
            
            src_x = src_loc['x'] + src_size['width'] // 2
            src_y = src_loc['y'] + src_size['height'] // 2
            tgt_x = tgt_loc['x'] + tgt_size['width'] // 2
            tgt_y = tgt_loc['y'] + tgt_size['height'] // 2
            
            self._perform_gesture(src_x, src_y, tgt_x, tgt_y, duration_ms=int(duration * 1000))
            return True
        return False
    
    def pinch(self, identifier: str, scale: float = 0.5):
        """
        Pinch gesture on element (zoom out).
        
        Args:
            identifier: Element identifier
            scale: Scale factor (< 1 = pinch in, > 1 = pinch out)
        """
        element = self.find_element(identifier)
        if element and self.platform == Platform.IOS:
            # iOS-specific pinch
            self.driver.execute_script('mobile: pinch', {
                'element': element,
                'scale': scale,
                'velocity': 1.0
            })
    
    # ==================== App Lifecycle ====================
    
    def activate_app(self, app_id: Optional[str] = None):
        """Bring app to foreground."""
        self.driver.activate_app(app_id or self.app_id)
    
    def terminate_app(self, app_id: Optional[str] = None) -> bool:
        """Terminate/close the app."""
        return self.driver.terminate_app(app_id or self.app_id)
    
    def install_app(self, app_path: str):
        """Install app from path (.app or .apk)."""
        self.driver.install_app(app_path)
    
    def remove_app(self, app_id: Optional[str] = None) -> bool:
        """Uninstall app."""
        return self.driver.remove_app(app_id or self.app_id)
    
    def is_app_installed(self, app_id: Optional[str] = None) -> bool:
        """Check if app is installed."""
        return self.driver.is_app_installed(app_id or self.app_id)
    
    def reset_app(self):
        """Reset app to clean state."""
        self.driver.reset()
    
    def background_app(self, seconds: int = -1):
        """
        Send app to background.
        
        Args:
            seconds: Seconds to stay in background (-1 = indefinitely)
        """
        self.driver.background_app(seconds)
    
    # ==================== Context Switching ====================
    
    def get_contexts(self) -> List[str]:
        """Get available contexts (NATIVE_APP, WEBVIEW_*, etc.)."""
        return self.driver.contexts
    
    def get_context(self) -> str:
        """Get current context."""
        return self.driver.context
    
    def switch_context(self, context: str):
        """
        Switch to different context.
        
        Args:
            context: Context name (e.g., 'NATIVE_APP', 'WEBVIEW_1')
        """
        self.driver.switch_to.context(context)
    
    def switch_to_webview(self) -> bool:
        """Switch to first available webview context."""
        contexts = self.get_contexts()
        for ctx in contexts:
            if 'WEBVIEW' in ctx:
                self.switch_context(ctx)
                return True
        return False
    
    def switch_to_native(self):
        """Switch back to native app context."""
        self.switch_context('NATIVE_APP')
    
    # ==================== Keyboard ====================
    
    def hide_keyboard(self):
        """Hide on-screen keyboard. Alias for dismiss_keyboard()."""
        return self.dismiss_keyboard()
    
    def is_keyboard_shown(self) -> bool:
        """Check if keyboard is visible."""
        try:
            return self.driver.is_keyboard_shown()
        except:
            return False
    
    def press_back(self):
        """Press back button (Android only)."""
        if self.platform == Platform.ANDROID:
            self.driver.back()
    
    def press_home(self):
        """Press home button."""
        if self.platform == Platform.ANDROID:
            self.driver.press_keycode(3)  # KEYCODE_HOME
        elif self.platform == Platform.IOS:
            self.driver.execute_script('mobile: pressButton', {'name': 'home'})
    
    # ==================== Device Actions ====================
    
    def get_orientation(self) -> str:
        """Get device orientation ('PORTRAIT' or 'LANDSCAPE')."""
        return self.driver.orientation
    
    def set_orientation(self, orientation: str):
        """
        Set device orientation.
        
        Args:
            orientation: 'PORTRAIT' or 'LANDSCAPE'
        """
        self.driver.orientation = orientation.upper()
    
    def get_window_size(self) -> Dict[str, int]:
        """Get window/screen size."""
        return self.driver.get_window_size()
    
    # ==================== Debug & Inspection ====================
    
    def screenshot(self, path: Optional[str] = None) -> str:
        """
        Take a screenshot.
        
        Args:
            path: File path to save (optional)
            
        Returns:
            File path if saved, or base64 string
        """
        if path:
            self.driver.save_screenshot(path)
            return path
        return self.driver.get_screenshot_as_base64()
    
    def page_source(self) -> str:
        """Get XML source of current page."""
        return self.driver.page_source
    
    def get_element_rect(self, identifier: str) -> Optional[Dict[str, int]]:
        """Get element's position and size."""
        element = self.find_element(identifier, timeout=2)
        if element:
            loc = element.location
            size = element.size
            return {
                'x': loc['x'],
                'y': loc['y'],
                'width': size['width'],
                'height': size['height']
            }
        return None
    
    def list_buttons(self) -> List[str]:
        """List all visible button labels (fast XML parsing)."""
        import xml.etree.ElementTree as ET
        
        if self.platform == Platform.IOS or self.platform == Platform.MACCATALYST:
            button_types = {"XCUIElementTypeButton"}
        else:
            button_types = {"android.widget.Button"}
        
        source = self.driver.page_source
        buttons = []
        
        try:
            root = ET.fromstring(source)
            for elem in root.iter():
                # Check if element is a button type
                if elem.tag in button_types or elem.get("class") in button_types:
                    label = (
                        elem.get("label") or 
                        elem.get("title") or 
                        elem.get("text") or 
                        elem.get("content-desc") or
                        elem.get("identifier") or 
                        elem.get("name") or
                        elem.get("value") or
                        ""
                    )
                    if label:
                        buttons.append(label)
        except:
            # Fallback to slow method
            return self._list_buttons_slow()
        
        return buttons
    
    def _list_buttons_slow(self) -> List[str]:
        """Slow button listing via Appium API calls (fallback)."""
        if self.platform == Platform.IOS or self.platform == Platform.MACCATALYST:
            class_name = "XCUIElementTypeButton"
        else:
            class_name = "android.widget.Button"
        
        elements = self.driver.find_elements(AppiumBy.CLASS_NAME, class_name)
        buttons = []
        for e in elements:
            try:
                label = None
                for attr in ["label", "title", "identifier", "name", "text", "value"]:
                    try:
                        label = e.get_attribute(attr)
                        if label:
                            break
                    except:
                        continue
                if not label:
                    label = e.text
                if label:
                    buttons.append(label)
            except:
                pass
        return buttons
    
    def _safe_get_attr(self, element, attr: str) -> Optional[str]:
        """Safely get attribute, handling platform differences."""
        try:
            return element.get_attribute(attr)
        except:
            return None
    
    def list_elements(self, limit: int = 100) -> List[Dict[str, Any]]:
        """List all elements with their identifiers (fast XML parsing)."""
        return self._list_elements_fast(limit)
    
    def _list_elements_fast(self, limit: int = 100) -> List[Dict[str, Any]]:
        """Fast element listing by parsing page source XML directly."""
        import xml.etree.ElementTree as ET
        
        source = self.driver.page_source
        result = []
        
        try:
            root = ET.fromstring(source)
        except ET.ParseError:
            # Fallback to slow method if XML parsing fails
            return self._list_elements_slow(limit)
        
        # Iterate through all elements in the XML
        for elem in root.iter():
            if len(result) >= limit:
                break
            
            # Extract relevant attributes based on platform
            info = {"type": elem.tag}
            
            # Common attributes across platforms
            attr_map = {
                # iOS/Mac Catalyst attributes
                "identifier": "identifier",
                "label": "label", 
                "value": "value",
                "title": "title",
                "name": "name",
                "placeholderValue": "placeholder",
                # Android attributes
                "resource-id": "resource-id",
                "content-desc": "content-desc",
                "text": "text",
            }
            
            for xml_attr, key in attr_map.items():
                val = elem.get(xml_attr)
                if val:
                    info[key] = val
            
            # Determine the display ID and text
            display_id = info.get("identifier") or info.get("resource-id") or info.get("name")
            display_text = info.get("label") or info.get("text") or info.get("value") or info.get("title")
            
            # Only include elements that have meaningful ID or text
            if display_id or display_text:
                result.append({
                    "type": elem.tag,
                    "id": display_id,
                    "text": display_text,
                })
        
        return result
    
    def _list_elements_slow(self, limit: int = 100) -> List[Dict[str, Any]]:
        """Slow element listing via Appium API calls (fallback)."""
        elements = self.driver.find_elements(AppiumBy.XPATH, "//*")
        result = []
        for e in elements[:limit]:
            try:
                info = {
                    "type": e.tag_name,
                    "enabled": e.is_enabled(),
                }
                # Platform-safe attribute access
                for attr in ["label", "identifier", "title", "value", "name", "text"]:
                    val = self._safe_get_attr(e, attr)
                    if val:
                        info[attr] = val
                try:
                    info["text"] = e.text
                except:
                    pass
                result.append(info)
            except:
                pass
        return result
    
    def find_by_text(self, text: str, partial: bool = False) -> List[Dict[str, Any]]:
        """
        Find all elements containing text.
        
        Args:
            text: Text to search for
            partial: Allow partial matches
        """
        if partial:
            xpath = f"//*[contains(@text, '{text}') or contains(@label, '{text}') or contains(@name, '{text}')]"
        else:
            xpath = f"//*[@text='{text}' or @label='{text}' or @name='{text}']"
        
        elements = self.driver.find_elements(AppiumBy.XPATH, xpath)
        result = []
        for e in elements:
            try:
                result.append({
                    "type": e.tag_name,
                    "text": e.text or e.get_attribute("label") or e.get_attribute("text"),
                    "accessibility_id": e.get_attribute("accessibility-id") or e.get_attribute("name"),
                })
            except:
                pass
        return result


# ==================== Device Management ====================

def list_ios_simulators() -> List[DeviceInfo]:
    """List all available iOS simulators."""
    result = subprocess.run(
        ["xcrun", "simctl", "list", "devices", "-j"],
        capture_output=True, text=True
    )
    data = json.loads(result.stdout)
    devices = []
    for runtime, device_list in data.get("devices", {}).items():
        if "iOS" in runtime:
            version = runtime.split("iOS-")[-1].replace("-", ".") if "iOS-" in runtime else None
            for device in device_list:
                devices.append(DeviceInfo(
                    udid=device["udid"],
                    name=device["name"],
                    platform=Platform.IOS,
                    version=version,
                    state=device.get("state", "unknown"),
                ))
    return devices


def list_android_devices() -> List[DeviceInfo]:
    """List connected Android devices and emulators."""
    result = subprocess.run(
        ["adb", "devices", "-l"],
        capture_output=True, text=True
    )
    devices = []
    lines = result.stdout.strip().split('\n')[1:]  # Skip header
    for line in lines:
        if '\tdevice' in line or 'device ' in line:
            parts = line.split()
            udid = parts[0]
            name = "Unknown"
            for part in parts:
                if part.startswith("model:"):
                    name = part.split(":")[1]
                    break
            devices.append(DeviceInfo(
                udid=udid,
                name=name,
                platform=Platform.ANDROID,
                state="device",
            ))
    return devices


def boot_ios_simulator(name_or_udid: str) -> str:
    """Boot an iOS simulator by name or UDID. Returns UDID."""
    # Check if already booted
    result = subprocess.run(
        ["xcrun", "simctl", "list", "devices", "booted", "-j"],
        capture_output=True, text=True
    )
    data = json.loads(result.stdout)
    for runtime, devices in data.get("devices", {}).items():
        for device in devices:
            if device["udid"] == name_or_udid or device["name"] == name_or_udid:
                return device["udid"]
    
    # Boot it
    subprocess.run(["xcrun", "simctl", "boot", name_or_udid], check=True)
    
    # Get UDID if we used name
    result = subprocess.run(
        ["xcrun", "simctl", "list", "devices", "booted", "-j"],
        capture_output=True, text=True
    )
    data = json.loads(result.stdout)
    for runtime, devices in data.get("devices", {}).items():
        for device in devices:
            if device["name"] == name_or_udid:
                return device["udid"]
    return name_or_udid


def shutdown_ios_simulator(udid: str):
    """Shutdown an iOS simulator."""
    subprocess.run(["xcrun", "simctl", "shutdown", udid])


def is_appium_running(url: str = "http://127.0.0.1:4723") -> bool:
    """Check if Appium server is running."""
    from urllib.parse import urlparse
    parsed = urlparse(url)
    host = parsed.hostname or "127.0.0.1"
    port = parsed.port or 4723
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    result = sock.connect_ex((host, port))
    sock.close()
    return result == 0


def start_appium(port: int = 4723, relaxed_security: bool = True) -> subprocess.Popen:
    """
    Start Appium server in background.
    
    Returns:
        Popen object for the server process
    """
    args = ["appium", "--port", str(port)]
    if relaxed_security:
        args.append("--relaxed-security")
    
    process = subprocess.Popen(
        args,
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        start_new_session=True
    )
    
    # Wait for server to be ready
    for _ in range(30):
        if is_appium_running(f"http://127.0.0.1:{port}"):
            return process
        time.sleep(0.5)
    
    raise RuntimeError("Appium server failed to start")


def parse_ordered_actions(argv: List[str]) -> List[Tuple[str, Any]]:
    """
    Parse command line arguments and return actions in the order they appear.
    
    Returns:
        List of (action_name, action_args) tuples in CLI order
    """
    actions = []
    i = 0
    
    # Define action argument counts
    action_args = {
        '--tap': 1, '--tap-button': 1, '--tap-text': 1, '--tap-like': 1,
        '--double-tap': 1, '--long-press': 1,
        '--type': 2, '--get-text': 1, '--exists': 1, '--expect': 2,
        '--clear': 1, '--is-enabled': 1, '--is-visible': 1,
        '--wait': 1, '--wait-for': 1,
        '--dismiss-keyboard': 0, '--press-key': 1,
        '--accept-alert': 0, '--dismiss-alert': 0, '--get-alert': 0,
        '--swipe': 1, '--scroll': 1, '--scroll-to': 1, '--tap-coords': 2,
        '--drag': -1,  # Variable args (3-4)
        '--set-slider': 2,
        '--activate': 0, '--terminate': 0, '--install': 1,
        '--screenshot': 1, '--page-source': 0,
        '--list-buttons': 0, '--list-elements': 0,
        '--find-text': 1, '--get-rect': 1,
    }
    
    while i < len(argv):
        arg = argv[i]
        if arg in action_args:
            num_args = action_args[arg]
            if num_args == 0:
                actions.append((arg, None))
                i += 1
            elif num_args == -1:  # Variable args (--drag)
                # Collect 3-4 args
                values = []
                j = i + 1
                while j < len(argv) and not argv[j].startswith('--') and len(values) < 4:
                    values.append(argv[j])
                    j += 1
                actions.append((arg, values))
                i = j
            else:
                values = argv[i+1:i+1+num_args]
                if len(values) == 1:
                    actions.append((arg, values[0]))
                else:
                    actions.append((arg, values))
                i += 1 + num_args
        else:
            i += 1
    
    return actions


# ==================== Action Handlers ====================

def _handle_simple_action(agent, method_name: str, value, success_msg: str, fail_msg: str):
    """Helper for simple element actions that return bool."""
    method = getattr(agent, method_name)
    success = method(value) if value else method()
    print(f"{success_msg}" if success else f"{fail_msg}")
    return True

def _handle_tap(agent, value):
    success = agent.tap(value)
    print(f"tap '{value}': {'ok' if success else 'not found'}")
    return True

def _handle_tap_button(agent, value):
    success = agent.tap_button(value)
    print(f"tap_button '{value}': {'ok' if success else 'not found'}")
    return True

def _handle_tap_text(agent, value):
    success = agent.tap_text(value)
    print(f"tap_text '{value}': {'ok' if success else 'not found'}")
    return True

def _handle_tap_like(agent, value):
    success = agent.tap_like(value)
    print(f"tap_like '{value}': {'ok' if success else 'not found'}")
    return True

def _handle_double_tap(agent, value):
    success = agent.double_tap(value)
    print(f"double_tap '{value}': {'ok' if success else 'not found'}")
    return True

def _handle_long_press(agent, value):
    success = agent.long_press(value)
    print(f"long_press '{value}': {'ok' if success else 'not found'}")
    return True

def _handle_type(agent, value):
    identifier, text = value
    success = agent.type_text(identifier, text)
    print(f"type '{identifier}' <- '{text}': {'ok' if success else 'not found'}")
    return True

def _handle_clear(agent, value):
    success = agent.clear(value)
    print(f"clear '{value}': {'ok' if success else 'not found'}")
    return True

def _handle_get_text(agent, value):
    text = agent.get_text(value)
    print(f"text '{value}': {text}")
    return True

def _handle_exists(agent, value):
    exists = agent.exists(value)
    print(f"exists '{value}': {exists}")
    return True

def _handle_expect(agent, value):
    identifier, expected = value
    passed, actual = agent.expect(identifier, expected)
    if passed:
        print(f"✓ expect '{identifier}' contains '{expected}': PASS (actual: {actual})")
        return True
    else:
        print(f"✗ expect '{identifier}' contains '{expected}': FAIL (actual: {actual})")
        return False

def _handle_is_enabled(agent, value):
    enabled = agent.is_enabled(value)
    print(f"is_enabled '{value}': {enabled if enabled is not None else 'not found'}")
    return True

def _handle_is_visible(agent, value):
    visible = agent.is_visible(value)
    print(f"is_visible '{value}': {visible}")
    return True

def _handle_wait(agent, value):
    import time
    secs = float(value)
    time.sleep(secs)
    print(f"wait: {secs}s")
    return True

def _handle_wait_for(agent, value):
    found = agent.wait_for(value, timeout=10)
    print(f"wait_for '{value}': {'found' if found else 'timeout'}")
    return True

def _handle_dismiss_keyboard(agent, value):
    success = agent.dismiss_keyboard()
    print(f"dismiss_keyboard: {'ok' if success else 'failed'}")
    return True

def _handle_press_key(agent, value):
    success = agent.press_key(value)
    print(f"press_key '{value}': {'ok' if success else 'failed'}")
    return True

def _handle_accept_alert(agent, value):
    success = agent.accept_alert()
    print(f"accept_alert: {'ok' if success else 'no alert'}")
    return True

def _handle_dismiss_alert(agent, value):
    success = agent.dismiss_alert()
    print(f"dismiss_alert: {'ok' if success else 'no alert'}")
    return True

def _handle_get_alert(agent, value):
    text = agent.get_alert_text()
    print(f"alert: {text}")
    return True

def _handle_swipe(agent, value):
    success = agent.swipe(value)
    print(f"swipe '{value}': {'ok' if success else 'failed'}")
    return True

def _handle_scroll(agent, value):
    success = agent.scroll(value)
    print(f"scroll '{value}': {'ok' if success else 'failed'}")
    return True

def _handle_scroll_to(agent, value):
    found = agent.scroll_to(value)
    print(f"scroll_to '{value}': {'found' if found else 'not found'}")
    return True

def _handle_tap_coords(agent, value):
    x, y = value
    agent.tap_coords(int(x), int(y))
    print(f"tap_coords: ({x}, {y})")
    return True

def _handle_drag(agent, value):
    if len(value) >= 3:
        identifier = value[0]
        offset_x = int(value[1])
        offset_y = int(value[2])
        duration = float(value[3]) if len(value) > 3 else 1.0
        success = agent.drag(identifier, offset_x, offset_y, duration)
        print(f"drag '{identifier}' by ({offset_x}, {offset_y}): {'ok' if success else 'failed'}")
    return True

def _handle_set_slider(agent, value):
    identifier, pct = value
    success = agent.set_slider(identifier, float(pct))
    print(f"set_slider '{identifier}' to {pct}%: {'ok' if success else 'failed'}")
    return True

def _handle_activate(agent, value):
    agent.activate_app()
    print("activate_app: ok")
    return True

def _handle_terminate(agent, value):
    agent.terminate_app()
    print("terminate_app: ok")
    return True

def _handle_install(agent, value):
    agent.install_app(value)
    print(f"install_app: {value}")
    return True

def _handle_screenshot(agent, value):
    agent.screenshot(value)
    print(f"screenshot: {value}")
    return True

def _handle_page_source(agent, value):
    print(agent.page_source())
    return True

def _handle_list_buttons(agent, value):
    print("buttons:")
    for btn in agent.list_buttons():
        print(f"  - {btn}")
    return True

def _handle_list_elements(agent, value):
    print("elements:")
    for elem in agent.list_elements(limit=50):
        eid = elem.get('id') or elem.get('accessibility_id')
        etext = elem.get('text')
        if eid or etext:
            print(f"  [{elem['type']}] id={eid} text={etext}")
    return True

def _handle_find_text(agent, value):
    print(f"elements with '{value}':")
    for elem in agent.find_by_text(value, partial=True):
        print(f"  [{elem['type']}] id={elem.get('accessibility_id')} text={elem.get('text')}")
    return True

def _handle_get_rect(agent, value):
    rect = agent.get_element_rect(value)
    if rect:
        print(f"rect '{value}': x={rect['x']} y={rect['y']} w={rect['width']} h={rect['height']}")
    else:
        print(f"rect '{value}': not found")
    return True


# Action registry: maps CLI argument to handler function
ACTION_HANDLERS = {
    '--tap': _handle_tap,
    '--tap-button': _handle_tap_button,
    '--tap-text': _handle_tap_text,
    '--tap-like': _handle_tap_like,
    '--double-tap': _handle_double_tap,
    '--long-press': _handle_long_press,
    '--type': _handle_type,
    '--clear': _handle_clear,
    '--get-text': _handle_get_text,
    '--exists': _handle_exists,
    '--expect': _handle_expect,
    '--is-enabled': _handle_is_enabled,
    '--is-visible': _handle_is_visible,
    '--wait': _handle_wait,
    '--wait-for': _handle_wait_for,
    '--dismiss-keyboard': _handle_dismiss_keyboard,
    '--press-key': _handle_press_key,
    '--accept-alert': _handle_accept_alert,
    '--dismiss-alert': _handle_dismiss_alert,
    '--get-alert': _handle_get_alert,
    '--swipe': _handle_swipe,
    '--scroll': _handle_scroll,
    '--scroll-to': _handle_scroll_to,
    '--tap-coords': _handle_tap_coords,
    '--drag': _handle_drag,
    '--set-slider': _handle_set_slider,
    '--activate': _handle_activate,
    '--terminate': _handle_terminate,
    '--install': _handle_install,
    '--screenshot': _handle_screenshot,
    '--page-source': _handle_page_source,
    '--list-buttons': _handle_list_buttons,
    '--list-elements': _handle_list_elements,
    '--find-text': _handle_find_text,
    '--get-rect': _handle_get_rect,
}


def execute_action(agent: 'AppiumAgent', action: str, value: Any) -> bool:
    """
    Execute a single action and print result.
    
    Returns:
        True if action succeeded, False if it failed (for assertions)
    """
    handler = ACTION_HANDLERS.get(action)
    if handler:
        return handler(agent, value)
    else:
        print(f"unknown action: {action}")
        return True


# ==================== CLI Interface ====================

def main():
    import argparse
    
    parser = argparse.ArgumentParser(
        description="Cross-Platform Mobile App Automation Agent",
        epilog="""
Examples:
  # iOS
  python appium_agent.py --platform ios --app-id com.example.app --tap MyButton
  
  # Android
  python appium_agent.py --platform android --app-id com.example.app --type EmailField "user@test.com"
  
  # Chain actions
  python appium_agent.py --platform ios --app-id com.example.app \\
    --tap LoginButton --wait 2 --get-text WelcomeLabel
  
  # List devices
  python appium_agent.py --list-devices
        """
    )
    
    # Device/platform options
    parser.add_argument("--platform", type=str, choices=["ios", "android", "maccatalyst"],
                        help="Target platform")
    parser.add_argument("--app-id", type=str, help="App identifier (bundle ID or package name)")
    parser.add_argument("--udid", type=str, help="Device UDID (auto-detects if not specified)")
    parser.add_argument("--appium-url", type=str, default="http://127.0.0.1:4723",
                        help="Appium server URL")
    parser.add_argument("--app-path", type=str, help="Path to .app/.apk to install")
    
    # Actions
    parser.add_argument("--tap", type=str, help="Tap element by identifier")
    parser.add_argument("--tap-button", type=str, help="Tap button by text")
    parser.add_argument("--tap-text", type=str, help="Tap element containing text")
    parser.add_argument("--tap-like", type=str, help="Tap element with partial ID match (fuzzy)")
    parser.add_argument("--double-tap", type=str, help="Double-tap element")
    parser.add_argument("--long-press", type=str, help="Long-press element")
    parser.add_argument("--type", nargs=2, metavar=("ID", "TEXT"), help="Type text into field")
    parser.add_argument("--clear", type=str, help="Clear text from field")
    parser.add_argument("--get-text", type=str, help="Get text from element")
    parser.add_argument("--exists", type=str, help="Check if element exists")
    parser.add_argument("--is-enabled", type=str, help="Check if element is enabled")
    parser.add_argument("--is-visible", type=str, help="Check if element is visible")
    parser.add_argument("--expect", nargs=2, metavar=("ID", "TEXT"), 
                        help="Assert element contains text (exits 1 if failed)")
    parser.add_argument("--wait", type=float, default=0, help="Wait seconds after action")
    parser.add_argument("--wait-for", type=str, help="Wait for element to appear")
    
    # Keyboard
    parser.add_argument("--dismiss-keyboard", action="store_true", help="Dismiss on-screen keyboard")
    parser.add_argument("--press-key", type=str, help="Press key (Enter, Tab, Backspace)")
    
    # Alerts
    parser.add_argument("--accept-alert", action="store_true", help="Accept/confirm alert dialog")
    parser.add_argument("--dismiss-alert", action="store_true", help="Dismiss/cancel alert dialog")
    parser.add_argument("--get-alert", action="store_true", help="Get alert text")
    
    # Gestures
    parser.add_argument("--swipe", type=str, choices=["up", "down", "left", "right"],
                        help="Swipe direction")
    parser.add_argument("--scroll", type=str, choices=["up", "down", "left", "right"],
                        help="Scroll in direction (without target)")
    parser.add_argument("--scroll-to", type=str, help="Scroll until element visible")
    parser.add_argument("--tap-coords", nargs=2, type=int, metavar=("X", "Y"),
                        help="Tap at coordinates")
    parser.add_argument("--drag", nargs='+', metavar="ARG",
                        help="Drag element by offset: ID OFFSET_X OFFSET_Y [DURATION]")
    parser.add_argument("--set-slider", nargs=2, metavar=("ID", "VALUE"),
                        help="Set slider value (0-1 or 0-100): --set-slider Slider 0.33")
    
    # App lifecycle
    parser.add_argument("--activate", action="store_true", help="Bring app to foreground")
    parser.add_argument("--terminate", action="store_true", help="Close app")
    parser.add_argument("--install", type=str, help="Install app from path")
    
    # Debug
    parser.add_argument("--screenshot", type=str, help="Save screenshot to path")
    parser.add_argument("--page-source", action="store_true", help="Print page source XML")
    parser.add_argument("--list-buttons", action="store_true", help="List all buttons")
    parser.add_argument("--list-elements", action="store_true", help="List all elements")
    parser.add_argument("--find-text", type=str, help="Find elements containing text")
    parser.add_argument("--get-rect", type=str, help="Get element position and size")
    
    # Device management
    parser.add_argument("--list-devices", action="store_true", help="List available devices")
    parser.add_argument("--boot-simulator", type=str, help="Boot iOS simulator by name/UDID")
    parser.add_argument("--start-appium", action="store_true", help="Start Appium server")
    
    # Session caching (performance)
    parser.add_argument("--keep-session", action="store_true", 
                       help="Keep session alive for reuse (faster subsequent calls)")
    parser.add_argument("--reuse-session", action="store_true",
                       help="Try to reuse a cached session")
    parser.add_argument("--end-session", action="store_true",
                       help="End all cached sessions")
    
    args = parser.parse_args()
    
    # Handle device management commands (no connection needed)
    if args.list_devices:
        print("iOS Simulators:")
        for d in list_ios_simulators():
            status = "✓ Booted" if d.state == "Booted" else ""
            print(f"  {d.name} ({d.udid[:8]}...) {d.version or ''} {status}")
        print("\nAndroid Devices:")
        try:
            for d in list_android_devices():
                print(f"  {d.name} ({d.udid})")
        except FileNotFoundError:
            print("  (adb not found)")
        return
    
    if args.boot_simulator:
        udid = boot_ios_simulator(args.boot_simulator)
        print(f"Booted: {udid}")
        return
    
    if args.start_appium:
        if is_appium_running(args.appium_url):
            print("Appium already running")
        else:
            start_appium()
            print("Appium started")
        return
    
    if args.end_session:
        AppiumAgent.end_all_sessions(args.appium_url)
        print("All cached sessions ended")
        return
    
    # Validate required args for automation commands
    if not args.platform or not args.app_id:
        parser.error("--platform and --app-id are required for automation commands")
    
    if not is_appium_running(args.appium_url):
        print("Appium not running, starting automatically...")
        try:
            start_appium()
            print("Appium started")
        except Exception as e:
            print(f"Error: Failed to start Appium: {e}")
            print("Start manually with: appium --relaxed-security")
            sys.exit(1)
    
    # Run automation
    with AppiumAgent(
        platform=args.platform,
        app_id=args.app_id,
        appium_url=args.appium_url,
        udid=args.udid,
        app_path=args.app_path,
        reuse_session=args.reuse_session,
        keep_session=args.keep_session,
    ) as agent:
        
        # Parse actions in CLI order and execute them
        ordered_actions = parse_ordered_actions(sys.argv)
        
        if not ordered_actions:
            print("No actions specified. Use --help for usage.")
            return
        
        for action, value in ordered_actions:
            success = execute_action(agent, action, value)
            if not success:
                # Assertion failed
                sys.exit(1)


if __name__ == "__main__":
    main()
