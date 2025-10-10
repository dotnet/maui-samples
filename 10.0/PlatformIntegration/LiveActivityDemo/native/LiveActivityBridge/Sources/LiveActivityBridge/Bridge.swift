import Foundation
import ActivityKit

private let kLAIdKey = "LiveActivityBridge.currentActivityId"

@_cdecl("LA_Start")
public func LA_Start(_ orderIdPtr: UnsafePointer<CChar>, _ titlePtr: UnsafePointer<CChar>, _ progress: Double) -> Bool {
    if #available(iOS 16.1, *) {
        guard ActivityAuthorizationInfo().areActivitiesEnabled else { return false }
        let orderId = String(cString: orderIdPtr)
        let title = String(cString: titlePtr)
        let attrs = DemoAttributes(orderId: orderId)
        let state = DemoAttributes.ContentState(title: title, progress: progress)
        do {
            let activity = try Activity.request(attributes: attrs, contentState: state, pushType: nil)
            UserDefaults.standard.set(activity.id, forKey: kLAIdKey)
            return true
        } catch {
            NSLog("LA_Start error: \(error)")
            return false
        }
    } else {
        return false
    }
}

@_cdecl("LA_Update")
public func LA_Update(_ titlePtr: UnsafePointer<CChar>, _ progress: Double) -> Bool {
    if #available(iOS 16.1, *) {
        let title = String(cString: titlePtr)
        let id = UserDefaults.standard.string(forKey: kLAIdKey)
        let activity = Activity<DemoAttributes>.activities.first { $0.id == id }
        guard let activity else { return false }
        let state = DemoAttributes.ContentState(title: title, progress: progress)
        Task { await activity.update(using: state) }
        return true
    } else {
        return false
    }
}

@_cdecl("LA_End")
public func LA_End() -> Bool {
    if #available(iOS 16.1, *) {
        let id = UserDefaults.standard.string(forKey: kLAIdKey)
        let activity = Activity<DemoAttributes>.activities.first { $0.id == id }
        guard let activity else { return false }
        Task { await activity.end(dismissalPolicy: .immediate) }
        UserDefaults.standard.removeObject(forKey: kLAIdKey)
        return true
    } else {
        return false
    }
}
