import { LastCardChanceModel } from "../../../models/LastCardChanceModel";

interface CardItemProps {
    card: LastCardChanceModel;
    onClick?: () => void;
}

export function CardItem({ card, onClick }: CardItemProps) {
    return (
        <a
            href="#"
            className="block block-link-pop text-center"
            onClick={(e) => {
                e.preventDefault();
                onClick?.();
            }}
        >
            <div className="block block-link-pop bg-xpro text-white h-100 mb-0">
                <div className="block-content text-center py-5">
                    <p className="mb-4">
                        <i className="fa fa-star fa-3x"></i>
                    </p>
                    <p className="fs-4 fw-bold mb-0">
                        {card.name.length > 15
                            ? <span title={card.name}>{card.name.slice(0, 15) + '...'}</span>
                            : card.name}
                    </p>
                </div>
            </div>
        </a>
    );
}