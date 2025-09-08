// swift-tools-version: 5.10
import PackageDescription

let package = Package(
    name: "LiveActivityBridge",
    platforms: [.iOS(.v16)],   // ⬅️ was .v16_1 (invalid in SPM)
    products: [
        .library(name: "LiveActivityBridge", type: .dynamic, targets: ["LiveActivityBridge"])
    ],
    targets: [
        .target(
            name: "LiveActivityBridge",
            path: "Sources"
        )
    ]
)
