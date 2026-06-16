/**
 * Address Drawer Manager - Standalone Final
 * مدیریت پنل کشویی آدرس‌ها بدون Bootstrap Offcanvas
 * نسخه کامل با تمام متدها و خطایابی
 */
class AddressDrawerManager {
    constructor(drawerId) {
        this.drawer = document.getElementById(drawerId);
        this.content = document.getElementById('addressDrawerContent');
        this.backdrop = null;
        this.currentView = 'list';
        this.isAnimating = false;
        this.isOpen = false;

        // بستن با دکمه ضربدر
        const closeBtn = this.drawer.querySelector('.btn-close');
        if (closeBtn) {
            closeBtn.addEventListener('click', () => this.close());
        }
    }

    open() {
        if (this.isOpen || this.isAnimating) return;
        this.isOpen = true;
        this.isAnimating = true;

        document.body.style.overflow = 'hidden';
        this._createBackdrop();

        this.drawer.style.visibility = 'visible';
        this.drawer.style.transition = 'transform 0.35s cubic-bezier(0.25, 0.8, 0.25, 1)';
        this.drawer.style.transform = 'translateX(0)';

        // بارگذاری لیست
        this.loadAddressList();

        setTimeout(() => { this.isAnimating = false; }, 350);
    }

    close() {
        if (!this.isOpen || this.isAnimating) return;
        this.isAnimating = true;

        this.drawer.style.transform = 'translateX(100%)';
        this._removeBackdrop();
        document.body.style.overflow = '';

        setTimeout(() => {
            this.drawer.style.visibility = 'hidden';
            this.isOpen = false;
            this.isAnimating = false;
        }, 350);
    }

    _createBackdrop() {
        if (this.backdrop) return;
        this.backdrop = document.createElement('div');
        this.backdrop.style.cssText = 'position:fixed;top:0;left:0;width:100%;height:100%;background:rgba(0,0,0,0.5);z-index:1040;transition:opacity 0.35s;';
        document.body.appendChild(this.backdrop);
        this.backdrop.addEventListener('click', () => this.close());
    }

    _removeBackdrop() {
        if (this.backdrop) {
            this.backdrop.style.opacity = '0';
            setTimeout(() => {
                if (this.backdrop?.parentNode) this.backdrop.parentNode.removeChild(this.backdrop);
                this.backdrop = null;
            }, 350);
        }
    }

    // بارگذاری لیست آدرس‌ها
    async loadAddressList() {
        await this._loadContent('/Account/AddressList', 'list');
    }

    // نمایش فرم افزودن
    async showAddForm() {
        await this._loadContent('/Account/AddAddress', 'add', () => this._parseValidation());
    }

    // نمایش فرم ویرایش
    async showEditForm(addressId) {
        await this._loadContent(`/Account/EditAddress?addressId=${addressId}`, 'edit', () => this._parseValidation());
    }

    // هستهٔ بارگذاری محتوا
    async _loadContent(url, view, afterLoad = null) {
        if (this.isAnimating && this.currentView === view) return;
        this.isAnimating = true;
        this._showSpinner();

        try {
            const response = await fetch(url);
            if (!response.ok) throw new Error(`خطای سرور (${response.status})`);
            const html = await response.text();

            // تشخیص View کامل
            if (html.includes('<html') || html.includes('<body')) {
                throw new Error('کنترلر یک صفحه کامل برگرداند. لطفاً PartialView استفاده کنید.');
            }

            // انیمیشن خروج محتوای قبلی
            const oldChild = this.content.firstElementChild;
            if (oldChild) {
                oldChild.style.transition = 'all 0.3s';
                oldChild.style.opacity = '0';
                oldChild.style.transform = 'translateX(-20px)';
            }

            await new Promise(resolve => setTimeout(resolve, 200));

            this.content.innerHTML = html;
            this.currentView = view;

            const newChild = this.content.firstElementChild;
            if (newChild) {
                newChild.style.opacity = '0';
                newChild.style.transform = 'translateX(20px)';
                newChild.style.transition = 'all 0.3s';
                requestAnimationFrame(() => {
                    newChild.style.opacity = '1';
                    newChild.style.transform = 'translateX(0)';
                });
            }

            // اتصال رویدادها
            if (view === 'list') this._attachListEvents();
            else if (view === 'add' || view === 'edit') this._attachFormEvents(view);

            if (afterLoad) afterLoad();
        } catch (error) {
            console.error(error);
            this.content.innerHTML = `
                <div class="alert alert-danger m-3">
                    <h6>خطا در بارگذاری</h6>
                    <p class="small">${error.message}</p>
                    <button class="btn btn-outline-danger btn-sm" onclick="addressDrawer.loadAddressList()">تلاش مجدد</button>
                </div>`;
        } finally {
            this.isAnimating = false;
        }
    }

    _attachListEvents() {
        // دکمه افزودن
        const addBtn = document.getElementById('addAddressBtn');
        if (addBtn) addBtn.addEventListener('click', () => this.showAddForm());

        // انتخاب آدرس
        const selectHandler = async (addressId) => {
            document.querySelectorAll('.address-card').forEach(c => c.classList.remove('selected'));
            const selectedCard = document.querySelector(`.address-card[data-address-id="${addressId}"]`);
            if (selectedCard) selectedCard.classList.add('selected');

            try {
                const response = await fetch(`/Home/SelectAddress?addressId=${addressId}`, { method: 'POST' });
                const data = await response.json();
                if (data.success) {
                    this.close();
                    document.body.style.transition = 'opacity 0.3s';
                    document.body.style.opacity = '0';
                    setTimeout(() => window.location.href = data.redirectUrl, 300);
                } else {
                    alert(data.message || 'خطا');
                    if (selectedCard) selectedCard.classList.remove('selected');
                }
            } catch {
                alert('خطا در ارتباط');
                if (selectedCard) selectedCard.classList.remove('selected');
            }
        };

        document.querySelectorAll('.select-address-btn').forEach(btn => {
            btn.addEventListener('click', (e) => selectHandler(e.currentTarget.dataset.addressId));
        });

        document.querySelectorAll('.address-card').forEach(card => {
            card.addEventListener('click', function (e) {
                if (!e.target.closest('button')) {
                    selectHandler(this.dataset.addressId);
                }
            });
        });

        // ویرایش
        document.querySelectorAll('.edit-address-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.stopPropagation();
                this.showEditForm(e.currentTarget.dataset.addressId);
            });
        });

        // حذف
        document.querySelectorAll('.delete-address-btn').forEach(btn => {
            btn.addEventListener('click', async (e) => {
                e.stopPropagation();
                const id = e.currentTarget.dataset.addressId;
                if (!confirm('مطمئن هستید؟')) return;
                const card = e.currentTarget.closest('.address-card');
                try {
                    const res = await fetch(`/Account/DeleteAddress?addressId=${id}`, { method: 'POST' });
                    if (res.ok) {
                        card.style.transition = 'all 0.3s';
                        card.style.opacity = '0';
                        card.style.transform = 'scale(0.95)';
                        setTimeout(() => {
                            card.remove();
                            if (document.querySelectorAll('.address-card').length === 0) this.loadAddressList();
                        }, 300);
                    } else {
                        alert('خطا در حذف');
                    }
                } catch {
                    alert('خطا در ارتباط');
                }
            });
        });
    }

    _attachFormEvents(mode) {
        const form = document.getElementById('addressForm');
        if (!form) return;

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            if (!$(form).valid()) return;

            const submitBtn = form.querySelector('button[type="submit"]');
            const originalText = submitBtn.innerHTML;
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>در حال ذخیره...';

            this._showSpinner();
            const formData = new FormData(form);
            try {
                const response = await fetch(form.action, { method: 'POST', body: formData });
                if (response.ok) {
                    this.content.innerHTML = `
                        <div class="text-center py-5" style="animation: fadeIn 0.5s;">
                            <i class="fas fa-check-circle text-success" style="font-size:4rem;animation:scaleIn 0.5s;"></i>
                            <h5 class="mt-3 fw-bold">${mode === 'edit' ? 'ویرایش شد' : 'ثبت شد'}</h5>
                        </div>`;
                    await new Promise(r => setTimeout(r, 1000));
                    await this.loadAddressList();
                } else {
                    const html = await response.text();
                    this.content.innerHTML = html;
                    this._attachFormEvents(mode);
                    this._parseValidation();
                }
            } catch {
                alert('خطا');
                this.loadAddressList();
            } finally {
                if (submitBtn) {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = originalText;
                }
            }
        });

        const backBtn = document.getElementById('backToListBtn');
        if (backBtn) backBtn.addEventListener('click', () => this.loadAddressList());
    }

    _parseValidation() {
        if ($.validator) {
            $('form').removeData('validator').removeData('unobtrusiveValidation');
            $.validator.unobtrusive.parse('form');
        }
    }

    _showSpinner() {
        this.content.innerHTML = `
            <div class="text-center py-5" style="animation:fadeIn 0.3s;">
                <div class="spinner-border text-success" style="width:3rem;height:3rem;" role="status">
                    <span class="visually-hidden">در حال بارگذاری...</span>
                </div>
                <p class="mt-2 text-muted small">لطفاً کمی صبر کنید...</p>
            </div>`;
    }
}