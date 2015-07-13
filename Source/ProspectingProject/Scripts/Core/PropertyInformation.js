

function buildPropertyInformationMenu() {
    return "";
}

function handlePropertyInformationMenuItemClick() {
    if (currentMarker) {
        return "Add valuation";
    } else {
        return "Please select a property to add a valuation to.";
    }
}