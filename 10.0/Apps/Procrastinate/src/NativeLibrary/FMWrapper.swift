import Foundation
import FoundationModels

/// Objective-C compatible wrapper for Apple's Foundation Models framework
/// Exposes key functionality to .NET MAUI via ObjCRuntime
@available(iOS 26.0, macOS 26.0, *)
@objc(FMWrapper)
public class FMWrapper: NSObject {
    
    // Shared state for async operation
    private static var _lastResult: String?
    private static var _lastError: String?
    private static var _isGenerating: Bool = false
    
    @objc public override init() {
        super.init()
    }
    
    /// Check if Foundation Models is available on this device
    @objc public static func isAvailable() -> Bool {
        let model = SystemLanguageModel.default
        switch model.availability {
        case .available:
            return true
        case .unavailable(_):
            return false
        @unknown default:
            return false
        }
    }
    
    /// Get the reason why Foundation Models is unavailable
    @objc public static func getUnavailabilityReason() -> String {
        let model = SystemLanguageModel.default
        switch model.availability {
        case .available:
            return ""
        case .unavailable(let reason):
            switch reason {
            case .deviceNotEligible:
                return "Device not eligible for Apple Intelligence"
            case .modelNotReady:
                return "Apple Intelligence model is not ready yet"
            case .appleIntelligenceNotEnabled:
                return "Enable Apple Intelligence in Settings"
            @unknown default:
                return "Unknown availability issue"
            }
        @unknown default:
            return "Unable to determine availability"
        }
    }
    
    /// Check if generation is in progress
    @objc public static func isGenerating() -> Bool {
        return _isGenerating
    }
    
    /// Get the last result (nil if not ready or error)
    @objc public static func getLastResult() -> String? {
        return _lastResult
    }
    
    /// Get the last error (nil if no error)
    @objc public static func getLastError() -> String? {
        return _lastError
    }
    
    /// Start generating text (async, poll with isGenerating/getLastResult)
    @objc public static func startGeneration(prompt: String, instructions: String) {
        guard !_isGenerating else { return }
        guard isAvailable() else {
            _lastError = getUnavailabilityReason()
            return
        }
        
        _isGenerating = true
        _lastResult = nil
        _lastError = nil
        
        Task {
            do {
                let session = LanguageModelSession(instructions: instructions)
                let response = try await session.respond(to: prompt)
                let content = response.content
                await MainActor.run {
                    _lastResult = content
                    _isGenerating = false
                }
            } catch let error as LanguageModelSession.GenerationError {
                let errorMessage: String
                switch error {
                case .exceededContextWindowSize:
                    errorMessage = "Prompt too long"
                case .guardrailViolation:
                    errorMessage = "Content policy violation"
                case .unsupportedLanguageOrLocale:
                    errorMessage = "Language not supported"
                case .rateLimited:
                    errorMessage = "Too many requests, try again"
                case .refusal:
                    errorMessage = "Request was refused"
                default:
                    errorMessage = "Generation failed: \(error.localizedDescription)"
                }
                await MainActor.run {
                    _lastError = errorMessage
                    _isGenerating = false
                }
            } catch {
                await MainActor.run {
                    _lastError = "Error: \(error.localizedDescription)"
                    _isGenerating = false
                }
            }
        }
    }
}
