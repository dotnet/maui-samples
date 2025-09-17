import ActivityKit
import SwiftUI
import WidgetKit
import LiveActivityBridge

struct LiveActivityWidget: Widget {
    var body: some WidgetConfiguration {
        ActivityConfiguration(for: DemoAttributes.self) { context in
            VStack(alignment: .leading) {
                Text("Order: \(context.attributes.orderId)").bold()
                Text(context.state.title)
                ProgressView(value: min(max(context.state.progress, 0.0), 1.0))
            }
            .padding()
            .activityBackgroundTint(Color.black.opacity(0.7))
            .activitySystemActionForegroundColor(.white)
        } dynamicIsland: { context in
            DynamicIsland {
                DynamicIslandExpandedRegion(.center) {
                    VStack {
                        Text(context.state.title)
                        ProgressView(value: min(max(context.state.progress, 0.0), 1.0))
                    }
                }
                DynamicIslandExpandedRegion(.trailing) {
                    Text(String(format: "%.0f%%", context.state.progress * 100))
                }
            } compactLeading: {
                Text(String(format: "%.0f%%", context.state.progress * 100))
            } compactTrailing: {
                Image(systemName: "bolt.fill")
            } minimal: {
                Image(systemName: "bolt.fill")
            }
        }
    }
}
