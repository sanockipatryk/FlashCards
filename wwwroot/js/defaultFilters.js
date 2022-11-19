var filtersForm = document.querySelector("#filtersForm");
var filterButton = document.querySelector("#filterButton");
var filtersAccordionButton = document.querySelector("#filtersAccordionButton");
var filtersAccordionText = document.querySelector(".filters-accordion-text");
var selectFields = document.querySelectorAll("#filtersForm select.form-select");

selectFields.forEach((select) => {
	setSelectTextColor(select);

	select.addEventListener("change", () => {
		setSelectTextColor(select);
	});
});

function setSelectTextColor(element) {
	if (element.value != element.querySelector("option:first-of-type").value) {
		if (!element.classList.contains("not-default"))
			element.classList.add("not-default");
	} else {
		if (element.classList.contains("not-default"))
			element.classList.remove("not-default");
	}
}

filtersAccordionButton.addEventListener("click", () => {
	if (filtersAccordionButton.classList.contains("collapsed")) {
		filtersAccordionText.textContent = "expand";
	} else {
		filtersAccordionText.textContent = "collapse";
	}
});

filtersForm.addEventListener("submit", (e) => {
	filtersForm.querySelectorAll("input").forEach((i) => {
		if (i.value.length === 0) {
			i.setAttribute("disabled", true);
		}
	});
	filtersForm.querySelectorAll("select").forEach((i) => {
		if (i.value.length === 0) {
			i.setAttribute("disabled", true);
		}
	});
});

filtersForm.addEventListener("keydown", (e) => {
	if (e.keyCode === 13) {
		e.preventDefault();
		filterButton.click();
	}
});

var clearFiltersButton = document.querySelector("#clearFiltersButton");
clearFiltersButton.addEventListener("click", () => {
	filtersForm.querySelectorAll("input").forEach((i) => {
		i.value = "";
	});
	filtersForm.querySelectorAll("select").forEach((i) => {
		i.value = i.querySelectorAll("option")[0].value;
	});
	filtersForm.submit();
});
