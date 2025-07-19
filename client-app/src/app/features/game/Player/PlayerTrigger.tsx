interface PlayerTriggerProps {
    playerName: string;
    onClick: () => void;
}

export function PlayerTrigger({ playerName, onClick }: PlayerTriggerProps) {
    return (
        <a
            href="#"
            className="block block-link-pop text-center"
            onClick={(e) => {
                e.preventDefault();
                onClick();
            }}
        >
            <div className="block block-link-pop bg-xpro text-white h-100 mb-0">
                <div className="block-content text-center py-5">
                    <p className="mb-4">
                        <i className="fa fa-user-check fa-3x"></i>
                    </p>
                    <p className="fs-4 fw-bold mb-0">
                        {playerName.length > 15
                            ? <span title={playerName}>{playerName.slice(0, 15) + '...'}</span>
                            : playerName}
                    </p>
                </div>
            </div>
        </a>
    );
}
