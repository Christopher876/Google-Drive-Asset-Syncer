#include "wintoastlib.h"
#include <string>     // std::string, std::stoi

using namespace WinToastLib;


class CustomHandler : public IWinToastHandler {
public:
    void toastActivated() const {
        std::wcout << L"The user clicked in this toast" << std::endl;      
    }

    void toastActivated(int actionIndex) const {
        std::wcout << L"The user clicked on action #" << actionIndex << std::endl;
    }

    void toastDismissed(WinToastDismissalReason state) const {
        switch (state) {
        case UserCanceled:
            std::wcout << L"The user dismissed this toast" << std::endl;
            exit(1);
            break;
        case TimedOut:
            std::wcout << L"The toast has timed out" << std::endl;
            exit(2);
            break;
        case ApplicationHidden:
            std::wcout << L"The application hid the toast using ToastNotifier.hide()" << std::endl;
            exit(3);
            break;
        default:
            std::wcout << L"Toast not activated" << std::endl;
            exit(4);
            break;
        }
    }

    void toastFailed() const {
        std::wcout << L"Error showing current toast" << std::endl;
        exit(5);
    }
};


enum Results {
	ToastClicked,					// user clicked on the toast
	ToastDismissed,					// user dismissed the toast
	ToastTimeOut,					// toast timed out
	ToastHided,						// application hid the toast
	ToastNotActivated,				// toast was not activated
	ToastFailed,					// toast failed
	SystemNotSupported,				// system does not support toasts
	UnhandledOption,				// unhandled option
	MultipleTextNotSupported,		// multiple texts were provided
	InitializationFailure,			// toast notification manager initialization failure
	ToastNotLaunched				// toast could not be launched
};

//Convert a string to wstring and cast to "LPCWSTR"
inline std::wstring to_wstring(const std::string& text) { return std::wstring(text.begin(), text.end());}

extern "C" {
    _declspec(dllexport) int Helloworld()
    {
        return 2;
    }
}

extern "C"
_declspec(dllexport) int WindowsNotify(const char *notification,const char *content, const char *name, const char *appModelID, const char *image = NULL)
{
    LPCWSTR appName = to_wstring(name).c_str(),
        appUserModelID = to_wstring(appModelID).c_str(),
        text = NULL,
        imagePath = NULL,
        attribute = NULL;
    std::vector<std::wstring> actions;
    INT64 expiration = 0;

    std::wstring insert = to_wstring(notification);
    std::wstring attrib = to_wstring(content);
    std::wstring img = to_wstring(image);

    text = insert.c_str();
    attribute = attrib.c_str();
    imagePath = img.c_str();

    WinToastTemplate::AudioOption audioOption = WinToastTemplate::AudioOption::Default;

    WinToast::instance()->setAppName(appName);
    WinToast::instance()->setAppUserModelId(appUserModelID);

    if (!text)
        text = L"Hello, world!";

    if (!WinToast::instance()->initialize())
    {
        std::wcerr << L"Error, your system in not compatible!" << std::endl;
        return Results::InitializationFailure;
    }

    bool withImage = (imagePath != NULL);
    WinToastTemplate templ(withImage ? WinToastTemplate::ImageAndText02 : WinToastTemplate::Text02);
    templ.setTextField(text, WinToastTemplate::FirstLine);
    templ.setAudioOption(audioOption);
    templ.setAttributionText(attribute);

    for (auto const &action : actions)
        templ.addAction(action);
    if (expiration)
        templ.setExpiration(expiration);
    if (withImage)
        templ.setImagePath(imagePath);


    if (WinToast::instance()->showToast(templ, new CustomHandler()) < 0)
    {
        std::wcerr << L"Could not launch your toast notification!";
        return Results::ToastFailed;
    }

    // Give the handler a chance for 15 seconds (or the expiration plus 1 second)
    Sleep(expiration ? (DWORD)expiration + 1000 : 15000);
}

/*int wmain()
{
    WindowsNotify("Tweedle","Dos");
}*/
