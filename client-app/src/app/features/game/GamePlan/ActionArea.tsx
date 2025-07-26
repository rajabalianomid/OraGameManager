import { useEffect, useState } from "react";
import { useStore } from "../../../Store";
import { AppConfig } from "../../../models/AppConfig";
import { RoleTrigger } from "../Player/RoleTrigger";
import { PlayerTrigger } from "../Player/PlayerTrigger";
import { ActiveCard } from "../Player/ActiveCard";

interface ActionAreaProps {
    roomId: string;
}

type ViewMode = "roles" | "abilities" | "players" | "none";

export default function ActionArea({ roomId }: ActionAreaProps) {

    const { communicationStore } = useStore();
    const userId = communicationStore.turnModel?.data?.userId ?? '';
    const abilities = communicationStore.turnModel?.data?.abilities || [];
    const roles = communicationStore.turnModel?.data?.roles || [];
    const cards = communicationStore.turnModel?.data?.cards || [];
    const players = communicationStore.turnModel?.data?.actingOn || [];
    const [viewMode, setViewMode] = useState<ViewMode>("none");
    const [hasUserInteracted, setHasUserInteracted] = useState(false);
    const [selectedAbility, setselectedAbility] = useState<string>("");

    useEffect(() => {
        if (!hasUserInteracted) {
            if (roles.length > 0) {
                setViewMode("roles");
            } else if (abilities.length > 0) {
                setViewMode("abilities");
            } else if (players.length > 0) {
                setViewMode("players");
            } else {
                setViewMode("none");
            }
        }
    }, [roles, abilities, players, hasUserInteracted]);


    const handleUseAbility = (abilityName: string, playerId: string) => {
        communicationStore.doAction(
            AppConfig.appId,
            roomId,
            userId,
            abilityName,
            playerId
        );
    };

    const handleBack = () => {
        setHasUserInteracted(true);
        setViewMode(roles.length > 0 ? "roles" : "abilities");
    };

    const handleClickToShowPlayers = () => {
        setHasUserInteracted(true);
        setViewMode("players");
    };

    const handleClickAbilityToShowPlayers = (ability: string) => {
        setHasUserInteracted(true);
        setselectedAbility(ability);
        setViewMode("players");
    };
    debugger;
    return (
        <>
            {viewMode === "roles" && (
                <div className="row g-sm">
                    {roles.map((role, i) => (
                        <div key={`role-${i}`} className="col-md-2 col-lg-2 col-xl-2">
                            <RoleTrigger roleName={role} onClick={handleClickToShowPlayers} />
                        </div>
                    ))}
                </div>
            )}

            {viewMode === "abilities" && (
                <div className="row g-sm">
                    {abilities.map((ability, i) => (
                        <div key={`ability-${i}`} className="col-md-2 col-lg-2 col-xl-2">
                            <a
                                href="#"
                                className="block block-link-pop text-center"
                                onClick={(e) => {
                                    e.preventDefault();
                                    handleClickAbilityToShowPlayers(ability.name);
                                }}
                            >
                                <div className="block block-link-pop bg-xpro text-white h-100 mb-0">
                                    <div className="block-content text-center py-5">
                                        <p className="mb-4">
                                            <i className={`fa fa-${ability.icon} fa-3x`}></i>
                                        </p>
                                        <p className="fs-4 fw-bold mb-0">
                                            {ability.name.length > 15
                                                ? <span title={ability.name}>{ability.name.slice(0, 15)}...</span>
                                                : ability.name}
                                        </p>
                                    </div>
                                </div>
                            </a>
                        </div>
                    ))}
                </div>
            )}

            {viewMode === "players" && (
                <>
                    <div className="text-center mb-3">
                        <a
                            className="btn btn-warning"
                            href="#"
                            onClick={(e) => {
                                e.preventDefault();
                                handleBack();
                            }}
                        >
                            <i className="fa fa-arrow-left me-2"></i>Back
                        </a>
                    </div>
                    {players.length === 0 ? (
                        <div className="text-center text-white fs-4 fw-bold mt-5">
                            No players found for this action.
                        </div>
                    ) : (
                        <div className="row g-sm">
                            {players.map((player, i) => (
                                <div key={`player-${i}`} className="col-md-2 col-lg-2 col-xl-2">
                                    <PlayerTrigger
                                        playerName={player.name}
                                        onClick={() => {
                                            const abilityName = selectedAbility; // ya select shode
                                            if (abilityName) {
                                                handleUseAbility(abilityName, player.userId);
                                            }
                                        }}
                                    />
                                </div>
                            ))}
                        </div>
                    )}
                </>
            )}

            <ActiveCard players={players} cards={cards} roomId={roomId} />
        </>
    );
}
