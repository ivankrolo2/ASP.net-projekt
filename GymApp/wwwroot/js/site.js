const sidebar = document.getElementById("mainSidebar");
const sidebarToggle = document.getElementById("sidebarToggle");

if (sidebar && sidebarToggle) {
	sidebarToggle.addEventListener("click", () => {
		sidebar.classList.toggle("open");
	});
}
