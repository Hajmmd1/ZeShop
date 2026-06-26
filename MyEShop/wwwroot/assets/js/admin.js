/**
 * Admin Panel – Unified Final
 */
document.addEventListener('DOMContentLoaded', function () {

    // ========== منوی موبایل ==========
    document.getElementById('menuToggle')?.addEventListener('click', function () {
        document.getElementById('adminSidebar')?.classList.toggle('open');
    });

    // ========== المان‌های مودال ==========
    const modal = document.getElementById('productModal');
    const openBtn = document.getElementById('openAddModal');
    const closeBtn = document.getElementById('modalCloseBtn');
    const cancelBtn = document.getElementById('modalCancelBtn');
    const form = document.getElementById('productForm');
    const modalTitle = document.getElementById('modalTitle');
    const imageInput = document.getElementById('productPicture');
    const imagePreview = document.getElementById('imagePreview');
    const imagePreviewContainer = document.getElementById('imagePreviewContainer');
    const fileNameSpan = document.getElementById('fileName');
    const toggleCategoryBtn = document.getElementById('toggleCategoryList');
    const categoryList = document.getElementById('categoryCheckboxList');
    const selectedContainer = document.getElementById('selectedCategoriesContainer');

    function showModal() { if (modal) modal.style.display = 'flex'; }
    window.closeModal = function () { if (modal) modal.style.display = 'none'; };

    function resetForm() {
        if (form) form.reset();
        if (imagePreviewContainer) imagePreviewContainer.style.display = 'none';
        if (fileNameSpan) fileNameSpan.textContent = 'هیچ فایلی انتخاب نشده';
        if (imageInput) imageInput.value = '';
        if (categoryList) categoryList.style.display = 'none';
    }

    function updateSelectedBadges() {
        if (!selectedContainer) return;
        const checked = document.querySelectorAll('.category-checkbox:checked');
        let html = '';
        checked.forEach(cb => {
            const label = document.querySelector(`label[for="cat_${cb.value}"]`);
            const name = label ? label.textContent : cb.value;
            html += `<span class="badge bg-success rounded-pill me-1 mb-1">${name}</span>`;
        });
        selectedContainer.innerHTML = html;
    }

    // ========== باز کردن مودال (افزودن) ==========
    if (openBtn) {
        openBtn.addEventListener('click', function () {
            if (modalTitle) modalTitle.textContent = 'افزودن محصول جدید';
            resetForm();
            const idInput = form?.querySelector('[name="AddOrEditProduct.Id"]');
            if (idInput) idInput.value = '0';
            document.querySelectorAll('.category-checkbox').forEach(cb => cb.checked = false);
            updateSelectedBadges();
            showModal();
        });
    }

    // ========== بستن مودال ==========
    if (closeBtn) closeBtn.addEventListener('click', window.closeModal);
    if (cancelBtn) cancelBtn.addEventListener('click', window.closeModal);
    if (modal) modal.addEventListener('click', function (e) { if (e.target === modal) window.closeModal(); });

    // ========== پیش‌نمایش تصویر ==========
    if (imageInput) {
        imageInput.addEventListener('change', function (e) {
            const file = e.target.files[0];
            if (!file) {
                if (fileNameSpan) fileNameSpan.textContent = 'هیچ فایلی انتخاب نشده';
                if (imagePreviewContainer) imagePreviewContainer.style.display = 'none';
                return;
            }
            if (fileNameSpan) fileNameSpan.textContent = file.name;
            const reader = new FileReader();
            reader.onload = function (ev) {
                if (imagePreview) imagePreview.src = ev.target.result;
                if (imagePreviewContainer) imagePreviewContainer.style.display = 'block';
            };
            reader.readAsDataURL(file);
        });
    }

    // ========== دسته‌بندی‌ها ==========
    if (toggleCategoryBtn && categoryList) {
        toggleCategoryBtn.addEventListener('click', function () {
            categoryList.style.display = categoryList.style.display === 'none' ? 'block' : 'none';
        });
    }
    document.querySelectorAll('.category-checkbox').forEach(cb => {
        cb.addEventListener('change', updateSelectedBadges);
    });

    // ========== حذف محصول ==========
    document.querySelectorAll('.delete-product-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.dataset.productId;
            if (confirm('آیا از حذف این محصول مطمئن هستید؟')) {
                fetch(`/Admin/Product?handler=Delete&id=${id}`, { method: 'POST' })
                    .then(res => {
                        if (res.ok) location.reload();
                        else alert('خطا در حذف');
                    })
                    .catch(() => alert('خطا در ارتباط'));
            }
        });
    });

    // ========== باز کردن خودکار مودال در حالت ویرایش (از طریق URL) ==========
    const urlParams = new URLSearchParams(window.location.search);
    const productId = urlParams.get('id');
    if (productId && parseInt(productId) > 0) {
        if (modalTitle) modalTitle.textContent = 'ویرایش محصول';
        if (modal) modal.style.display = 'flex';
        // فرم ریست نمی‌شود – مقادیر از OnGet پر شده‌اند
    }

});