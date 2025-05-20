import { usePropsFor, VideoGallery, ControlBar, CameraButton, MicrophoneButton, ScreenShareButton, EndCallButton, useCall } from "@azure/communication-react";
import { useStore } from "../../Store";
import { observer } from "mobx-react-lite";

function CallingComponents() {

    const { communicationStore } = useStore();

    const videoGalleryProps = usePropsFor(VideoGallery);
    const cameraProps = usePropsFor(CameraButton);
    const microphoneProps = usePropsFor(MicrophoneButton);
    const screenShareProps = usePropsFor(ScreenShareButton);
    const endCallProps = usePropsFor(EndCallButton);

    const call = useCall();

    const buttonsDisabled = !(
        call?.state === "InLobby" || call?.state === "Connected"
    );

    if (call?.state === "Disconnected") {
        communicationStore.setOnCall(false);
        return <CallEnded />;
    }

    return (
        <div className="relative flex flex-col h-full bg-gray-50">
            <div className="h-full w-full pb-[32px]">
                {videoGalleryProps && (
                    <VideoGallery
                        {...videoGalleryProps}
                        localVideoViewOptions={{
                            isMirrored: true,
                            scalingMode: "Fit",
                        }}
                        remoteVideoViewOptions={{
                            scalingMode: "Fit",
                        }}
                    />
                )}
            </div>

            <div className="absolute bottom-0 left-1/2 transform -translate-x-1/2">
                <ControlBar
                    styles={{
                        root: {
                            height: "32px",
                            width: "100%",
                            backgroundColor: "rgb(249 250 251)",
                            "& button": {
                                height: "32px",
                                width: "32px",
                                padding: 0,
                                minWidth: "32px",
                                minHeight: "32px",
                            },
                        },
                    }}
                >
                    {cameraProps && (
                        <CameraButton
                            size={5}
                            {...cameraProps}
                            disabled={buttonsDisabled ?? cameraProps.disabled}
                        />
                    )}
                    {microphoneProps && (
                        <MicrophoneButton
                            {...microphoneProps}
                            disabled={buttonsDisabled ?? microphoneProps.disabled}
                        />
                    )}
                    {screenShareProps && (
                        <ScreenShareButton
                            {...screenShareProps}
                            disabled={buttonsDisabled}
                        />
                    )}
                    {endCallProps && (
                        <EndCallButton
                            styles={{
                                root: {
                                    color: "black",
                                },
                            }}
                            {...endCallProps}
                            disabled={buttonsDisabled}
                        />
                    )}
                </ControlBar>
            </div>
        </div>
    );
};

const CallEnded = () => {
    return <h1>You ended the call.</h1>;
};

export default observer(CallingComponents);