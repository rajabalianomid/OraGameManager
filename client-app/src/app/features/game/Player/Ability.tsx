interface AbilityModel {
    name: string;
    icon: string;
}

interface AbilityListProps {
    abilities: AbilityModel[];
    onBack: () => void;
    onUseAbility: (abilityName: string) => void;
}

export function Ability({ abilities, onBack, onUseAbility }: AbilityListProps) {
    return (
        <>
            {abilities.map((ability, idx) => (
                <div key={idx}>
                    <a className="block text-center bg-primary" onClick={() => onUseAbility(ability.name)}>
                        <div className="block-content block-content-full ratio ratio-16x9">
                            <div className="d-flex justify-content-center align-items-center">
                                <div>
                                    <i className={`far fa-2x fa-${ability.icon} text-primary-lighter`}></i>
                                    <div className="fw-semibold mt-3 text-uppercase text-white">{ability.name}</div>
                                </div>
                            </div>
                        </div>
                    </a>
                    <a className="block text-center bg-xmodern" href="#" onClick={onBack}>
                        <div className="block-content block-content-full ratio">
                            <div className="d-flex justify-content-center align-items-center">
                                <div>
                                    <i className="fa fa-2x fa-arrow-left text-xmodern-lighter"></i>
                                </div>
                            </div>
                        </div>
                    </a>
                </div>
            ))}
        </>
    );
}