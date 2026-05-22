const sidebar = document.getElementById("mainSidebar");
const sidebarToggle = document.getElementById("sidebarToggle");

if (sidebar && sidebarToggle) {
	sidebarToggle.addEventListener("click", () => {
		sidebar.classList.toggle("open");
	});
}

const searchInputs = document.querySelectorAll("[data-search-input]");

const buildSearchUrl = (baseUrl, params) => {
	const url = new URL(baseUrl, window.location.origin);
	Object.entries(params).forEach(([key, value]) => {
		if (value !== null && value !== undefined && value !== "") {
			url.searchParams.set(key, value);
		}
	});
	return url.toString();
};

searchInputs.forEach((input) => {
	const targetId = input.dataset.searchTarget;
	const target = targetId ? document.getElementById(targetId) : null;
	const searchUrl = input.dataset.searchUrl;
	const paramName = input.dataset.searchParam || "q";
	const filterName = input.dataset.filterName;
	const filterValue = input.dataset.filterValue;

	if (!target || !searchUrl) {
		return;
	}

	let debounceTimer;
	const runSearch = async () => {
		const params = {
			[paramName]: input.value.trim()
		};

		if (filterName && filterValue) {
			params[filterName] = filterValue;
		}

		try {
			const url = buildSearchUrl(searchUrl, params);
			const response = await fetch(url, {
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				}
			});
			if (!response.ok) {
				return;
			}

			const html = await response.text();
			target.innerHTML = html;
		} catch (error) {
			console.error("Search failed", error);
		}
	};

	input.addEventListener("input", () => {
		window.clearTimeout(debounceTimer);
		debounceTimer = window.setTimeout(runSearch, 300);
	});
});

const autocompleteInputs = document.querySelectorAll("[data-autocomplete-input]");

const closeAllAutocomplete = () => {
	const menus = document.querySelectorAll("[data-autocomplete-menu]");
	menus.forEach((menu) => {
		menu.innerHTML = "";
		menu.classList.remove("open");
	});
};

const createAutocompleteItem = (item, onSelect) => {
	const button = document.createElement("button");
	button.type = "button";
	button.className = "autocomplete-item";
	button.textContent = item.label;
	if (item.meta) {
		const meta = document.createElement("span");
		meta.className = "autocomplete-meta";
		meta.textContent = item.meta;
		button.appendChild(meta);
	}
	button.addEventListener("click", () => onSelect(item));
	return button;
};

autocompleteInputs.forEach((input) => {
	const hiddenId = input.dataset.autocompleteTarget;
	const hidden = hiddenId ? document.getElementById(hiddenId) : null;
	const url = input.dataset.autocompleteUrl;
	const menu = input.closest(".autocomplete")?.querySelector("[data-autocomplete-menu]");

	if (!hidden || !url || !menu) {
		return;
	}

	let debounceTimer;
	const runAutocomplete = async () => {
		const query = input.value.trim();
		if (!query) {
			menu.innerHTML = "";
			menu.classList.remove("open");
			return;
		}

		try {
			const response = await fetch(buildSearchUrl(url, { q: query }), {
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				}
			});
			if (!response.ok) {
				return;
			}

			const items = await response.json();
			menu.innerHTML = "";
			if (!items.length) {
				const empty = document.createElement("div");
				empty.className = "autocomplete-empty";
				empty.textContent = "Nema rezultata";
				menu.appendChild(empty);
				menu.classList.add("open");
				return;
			}

			items.forEach((item) => {
				menu.appendChild(createAutocompleteItem(item, (selected) => {
					hidden.value = selected.id;
					input.value = selected.label;
					menu.innerHTML = "";
					menu.classList.remove("open");
				}));
			});
			menu.classList.add("open");
		} catch (error) {
			console.error("Autocomplete failed", error);
		}
	};

	input.addEventListener("input", () => {
		hidden.value = "";
		window.clearTimeout(debounceTimer);
		debounceTimer = window.setTimeout(runAutocomplete, 250);
	});

	input.addEventListener("focus", () => {
		if (input.value.trim()) {
			runAutocomplete();
		}
	});

	input.addEventListener("blur", () => {
		if (window.jQuery && hidden.classList.contains("validate-hidden")) {
			window.jQuery(hidden).valid();
		}
	});
});

document.addEventListener("click", (event) => {
	if (!event.target.closest(".autocomplete")) {
		closeAllAutocomplete();
	}
});

if (window.jQuery && window.jQuery.validator) {
	window.jQuery.validator.setDefaults({
		ignore: ":hidden:not(.validate-hidden)",
		onfocusout(element) {
			this.element(element);
		}
	});
}

const dateTimePickers = document.querySelectorAll("[data-date-time-picker]");

const getLocale = () => navigator.language || document.documentElement.lang || "en";

const formatDisplay = (date, locale) => {
	const datePart = new Intl.DateTimeFormat(locale, {
		year: "numeric",
		month: "2-digit",
		day: "2-digit"
	}).format(date);
	const timePart = new Intl.DateTimeFormat(locale, {
		hour: "2-digit",
		minute: "2-digit",
		hour12: false
	}).format(date);
	return `${datePart} ${timePart}`;
};

const parseHiddenValue = (value) => {
	if (!value) {
		return null;
	}
	const date = new Date(value);
	if (Number.isNaN(date.getTime())) {
		return null;
	}
	if (date.getFullYear() < 1901) {
		return null;
	}
	return date;
};

const normalizeNumber = (value, min, max) => {
	const parsed = Number.parseInt(value, 10);
	if (Number.isNaN(parsed)) {
		return min;
	}
	return Math.min(Math.max(parsed, min), max);
};

const buildWeekdays = (container, locale) => {
	container.innerHTML = "";
	const base = new Date(Date.UTC(2020, 4, 31));
	const formatter = new Intl.DateTimeFormat(locale, { weekday: "short" });
	for (let i = 0; i < 7; i += 1) {
		const day = new Date(base.getTime() + i * 86400000);
		const cell = document.createElement("div");
		cell.className = "date-time-weekday";
		cell.textContent = formatter.format(day);
		container.appendChild(cell);
	}
};

const buildCalendar = (grid, state) => {
	grid.innerHTML = "";
	const { current, selected } = state;
	const year = current.getFullYear();
	const month = current.getMonth();
	const firstDay = new Date(year, month, 1);
	const startDay = new Date(year, month, 1 - firstDay.getDay());
	for (let i = 0; i < 42; i += 1) {
		const date = new Date(startDay);
		date.setDate(startDay.getDate() + i);
		const button = document.createElement("button");
		button.type = "button";
		button.className = "date-time-day";
		button.textContent = date.getDate().toString();
		if (date.getMonth() !== month) {
			button.classList.add("is-muted");
		}
		if (selected && date.toDateString() === selected.toDateString()) {
			button.classList.add("is-selected");
		}
		button.dataset.dateTimeValue = date.toISOString();
		grid.appendChild(button);
	}
};

const updateTitle = (title, date, locale) => {
	title.textContent = new Intl.DateTimeFormat(locale, {
		month: "long",
		year: "numeric"
	}).format(date);
};

const updateDisplay = (display, hidden, date, locale) => {
	if (!date) {
		display.value = "";
		hidden.value = "";
		return;
	}
	const iso = new Date(date.getTime() - date.getTimezoneOffset() * 60000)
		.toISOString()
		.slice(0, 16);
	hidden.value = iso;
	display.value = formatDisplay(date, locale);
};

dateTimePickers.forEach((picker) => {
	const display = picker.querySelector("[data-date-time-display]");
	const hidden = picker.querySelector("[data-date-time-value]");
	const panel = picker.querySelector("[data-date-time-panel]");
	const title = picker.querySelector("[data-date-time-title]");
	const weekdays = picker.querySelector("[data-date-time-weekdays]");
	const grid = picker.querySelector("[data-date-time-grid]");
	const hourInput = picker.querySelector("[data-date-time-hour]");
	const minuteInput = picker.querySelector("[data-date-time-minute]");
	const prevBtn = picker.querySelector("[data-date-time-prev]");
	const nextBtn = picker.querySelector("[data-date-time-next]");
	const todayBtn = picker.querySelector("[data-date-time-today]");
	const clearBtn = picker.querySelector("[data-date-time-clear]");
	const applyBtn = picker.querySelector("[data-date-time-apply]");
	const locale = getLocale();

	if (!display || !hidden || !panel || !title || !weekdays || !grid || !hourInput || !minuteInput) {
		return;
	}

	const initial = parseHiddenValue(hidden.value) || null;
	const state = {
		current: initial ? new Date(initial) : new Date(),
		selected: initial ? new Date(initial) : null
	};

	buildWeekdays(weekdays, locale);
	updateTitle(title, state.current, locale);
	buildCalendar(grid, state);
	if (state.selected) {
		hourInput.value = state.selected.getHours().toString().padStart(2, "0");
		minuteInput.value = state.selected.getMinutes().toString().padStart(2, "0");
		updateDisplay(display, hidden, state.selected, locale);
	} else {
		updateDisplay(display, hidden, null, locale);
		hourInput.value = "00";
		minuteInput.value = "00";
	}

	const openPanel = () => {
		panel.classList.add("open");
	};

	const closePanel = () => {
		panel.classList.remove("open");
	};

	display.addEventListener("click", (event) => {
		event.stopPropagation();
		panel.classList.toggle("open");
	});

	prevBtn.addEventListener("click", () => {
		state.current.setMonth(state.current.getMonth() - 1);
		updateTitle(title, state.current, locale);
		buildCalendar(grid, state);
	});

	nextBtn.addEventListener("click", () => {
		state.current.setMonth(state.current.getMonth() + 1);
		updateTitle(title, state.current, locale);
		buildCalendar(grid, state);
	});

	grid.addEventListener("click", (event) => {
		const target = event.target;
		if (!(target instanceof HTMLElement)) {
			return;
		}
		const value = target.dataset.dateTimeValue;
		if (!value) {
			return;
		}
		const picked = new Date(value);
		if (!state.selected) {
			state.selected = picked;
		} else {
			state.selected.setFullYear(picked.getFullYear(), picked.getMonth(), picked.getDate());
		}
		buildCalendar(grid, state);
	});

	todayBtn.addEventListener("click", () => {
		const now = new Date();
		state.current = new Date(now);
		state.selected = new Date(now);
		hourInput.value = now.getHours().toString().padStart(2, "0");
		minuteInput.value = now.getMinutes().toString().padStart(2, "0");
		updateTitle(title, state.current, locale);
		buildCalendar(grid, state);
		updateDisplay(display, hidden, state.selected, locale);
		closePanel();
	});

	clearBtn.addEventListener("click", () => {
		state.selected = null;
		updateDisplay(display, hidden, null, locale);
		buildCalendar(grid, state);
		closePanel();
	});

	applyBtn.addEventListener("click", () => {
		const hours = normalizeNumber(hourInput.value, 0, 23);
		const minutes = normalizeNumber(minuteInput.value, 0, 59);
		if (!state.selected) {
			state.selected = new Date();
		}
		state.selected.setHours(hours, minutes, 0, 0);
		updateDisplay(display, hidden, state.selected, locale);
		if (window.jQuery && hidden.classList.contains("validate-hidden")) {
			window.jQuery(hidden).valid();
		}
		closePanel();
	});

	[hourInput, minuteInput].forEach((input) => {
		input.addEventListener("blur", () => {
			input.value = normalizeNumber(input.value, 0, input === hourInput ? 23 : 59)
				.toString()
				.padStart(2, "0");
		});
	});

	document.addEventListener("click", (event) => {
		if (!picker.contains(event.target)) {
			closePanel();
		}
	});
});

const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

const animateCount = (element) => {
	const target = Number(element.dataset.count);
	if (Number.isNaN(target)) {
		return;
	}

	const duration = 700;
	const start = performance.now();
	const formatValue = (value) => Math.round(value).toString();

	const tick = (now) => {
		const progress = Math.min((now - start) / duration, 1);
		const value = target * (1 - Math.pow(1 - progress, 3));
		element.textContent = formatValue(value);
		if (progress < 1) {
			requestAnimationFrame(tick);
		}
	};

	requestAnimationFrame(tick);
};

const animateProgress = (element) => {
	const raw = Number(element.dataset.progress);
	if (Number.isNaN(raw)) {
		return;
	}

	const clamped = Math.max(0, Math.min(raw, 100));
	element.style.width = `${clamped}%`;
};

const animateOnView = (entries, observer) => {
	entries.forEach((entry) => {
		if (!entry.isIntersecting) {
			return;
		}

		entry.target.classList.add("in-view");
		const counters = entry.target.querySelectorAll("[data-count]");
		counters.forEach((counter) => {
			if (!counter.dataset.animated) {
				counter.dataset.animated = "true";
				if (prefersReducedMotion) {
					counter.textContent = counter.dataset.count;
				} else {
					animateCount(counter);
				}
			}
		});

		const progressBars = entry.target.querySelectorAll("[data-progress]");
		progressBars.forEach((bar) => {
			if (!bar.dataset.animated) {
				bar.dataset.animated = "true";
				if (!prefersReducedMotion) {
					animateProgress(bar);
				} else {
					bar.style.width = `${bar.dataset.progress}%`;
				}
			}
		});

		observer.unobserve(entry.target);
	});
};

const animatedSections = document.querySelectorAll("[data-animate]");
if (animatedSections.length) {
	if (prefersReducedMotion) {
		animatedSections.forEach((section) => section.classList.add("in-view"));
		const counters = document.querySelectorAll("[data-count]");
		counters.forEach((counter) => {
			counter.textContent = counter.dataset.count;
		});
		const progressBars = document.querySelectorAll("[data-progress]");
		progressBars.forEach((bar) => {
			bar.style.width = `${bar.dataset.progress}%`;
		});
	} else {
		const observer = new IntersectionObserver(animateOnView, {
			threshold: 0.2
		});
		animatedSections.forEach((section) => observer.observe(section));
	}
}
