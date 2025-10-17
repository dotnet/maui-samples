import Foundation
import ActivityKit

public struct DemoAttributes: ActivityAttributes {
    public struct ContentState: Codable, Hashable {
        public var title: String
        public var progress: Double
        public init(title: String, progress: Double) {
            self.title = title
            self.progress = progress
        }
    }
    public var orderId: String
    public init(orderId: String) {
        self.orderId = orderId
    }
}
