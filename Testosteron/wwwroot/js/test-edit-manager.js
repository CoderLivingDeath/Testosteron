// TestEditManager - управление редактированием теста
class TestEditManager {
    constructor(options = {}) {
        this.fieldsContainer = document.getElementById(options.fieldsContainerId || 'fieldsContainer');
        this.addFieldBtn = document.getElementById(options.addFieldBtnId || 'addField');
        this.form = document.getElementById(options.formId || 'TestForm');
        this.resultDiv = document.getElementById(options.resultDivId || 'resultDiv');

        this.templateUrl = options.templateUrl || '/Admin/Test/GetFieldTemplate';
        this.initialFieldCount = options.initialFieldCount || 0;

        this.onSuccess = options.onSuccess || this._defaultOnSuccess.bind(this);
        this.onError = options.onError || this._defaultOnError.bind(this);

        this.fieldCount = this.initialFieldCount;
    }

    init() {
        if (!this.fieldsContainer || !this.form) {
            console.error('TestEditManager: Required elements not found');
            return this;
        }

        this._bindEvents();
        this._initEditableSync();

        return this;
    }

    _bindEvents() {
        this.fieldsContainer.addEventListener('click', (e) => this._handleContainerClick(e));
        this.addFieldBtn?.addEventListener('click', () => this.addField());
        this.form.addEventListener('submit', (e) => this.handleSubmit(e));
    }

    async addField() {
        try {
            const tempIndex = '__NEW__' + Date.now();
            const response = await fetch(this.templateUrl + '?index=' + encodeURIComponent(tempIndex));
            if (!response.ok) throw new Error('Failed to load template');

            const html = await response.text();
            this.fieldsContainer.insertAdjacentHTML('beforeend', html);

            const newField = this.fieldsContainer.lastElementChild;
            this._triggerEvent('fieldAdded', { field: newField });

            return newField;
        } catch (error) {
            this.onError(error);
            return null;
        }
    }

    removeField(fieldElement) {
        if (!fieldElement) return false;
        fieldElement.remove();
        this.reindexAllFields();
        this._triggerEvent('fieldRemoved', { field: fieldElement });
        return true;
    }

    moveUp(fieldElement) {
        if (!fieldElement) return false;
        const prev = fieldElement.previousElementSibling;
        if (prev) {
            fieldElement.parentNode.insertBefore(fieldElement, prev);
            this.reindexAllFields();
            return true;
        }
        return false;
    }

    moveDown(fieldElement) {
        if (!fieldElement) return false;
        const next = fieldElement.nextElementSibling;
        if (next) {
            fieldElement.parentNode.insertBefore(next, fieldElement);
            this.reindexAllFields();
            return true;
        }
        return false;
    }

    addOption(fieldElement) {
        if (!fieldElement) return null;
        const container = fieldElement.querySelector('.options-container');
        if (!container) return null;

        const optionCount = container.querySelectorAll('.option-row').length;
        const fieldIndex = this._getFieldIndex(fieldElement);
        const html = this._createOptionHtml(fieldIndex, optionCount);
        container.insertAdjacentHTML('beforeend', html);

        return container.lastElementChild;
    }

    removeOption(optionRow) {
        if (!optionRow) return false;
        const container = optionRow.closest('.options-container');
        optionRow.remove();
        if (container) {
            this.reindexOptions(container);
        }
        return true;
    }

    reindexAllFields() {
        const fields = this.fieldsContainer.querySelectorAll('.field-item');
        let newIndex = 0;

        fields.forEach(field => {
            field.querySelectorAll('[name]').forEach(input => {
                input.name = this._updateFieldIndex(input.name, newIndex);
            });
            this.reindexOptions(field.querySelector('.options-container'));
            newIndex++;
        });

        this.fieldCount = newIndex;
        return newIndex;
    }

    reindexOptions(optionsContainer) {
        if (!optionsContainer) return;
        const rows = optionsContainer.querySelectorAll('.option-row');
        const fieldElement = optionsContainer.closest('.field-item');
        const fieldIndex = this._getFieldIndex(fieldElement);

        rows.forEach((row, optIndex) => {
            row.querySelectorAll('[name]').forEach(input => {
                input.name = this._updateOptionIndex(input.name, fieldIndex, optIndex);
            });
        });
    }

    async handleSubmit(event) {
        event.preventDefault();

        this._syncAllEditables();
        this.reindexAllFields();

        const formData = new FormData(this.form);

        try {
            const response = await fetch(this.form.action, {
                method: 'PATCH',
                body: formData
            });

            const result = await response.json();
            result.success ? this.onSuccess(result) : this.onError(result);
            return result;
        } catch (error) {
            this.onError(error);
            return { success: false, message: error.message };
        }
    }

    _handleContainerClick(event) {
        const fieldItem = event.target.closest('.field-item');
        if (!fieldItem) return;

        if (event.target.classList.contains('delete-field')) {
            this.removeField(fieldItem);
        } else if (event.target.classList.contains('move-up')) {
            this.moveUp(fieldItem);
        } else if (event.target.classList.contains('move-down')) {
            this.moveDown(fieldItem);
        } else if (event.target.classList.contains('add-option')) {
            this.addOption(fieldItem);
        } else if (event.target.classList.contains('remove-option')) {
            this.removeOption(event.target.closest('.option-row'));
        }
    }

    _initEditableSync() {
        const titleEditable = this.form.querySelector('h1[contenteditable="true"]');
        const titleInput = document.getElementById('testTitleInput');
        if (titleEditable && titleInput) {
            titleEditable.addEventListener('input', () => {
                titleInput.value = titleEditable.innerText.trim();
            });
        }

        const descEditable = this.form.querySelector('div[contenteditable="true"]');
        const descInput = document.getElementById('testDescriptionInput');
        if (descEditable && descInput) {
            descEditable.addEventListener('input', () => {
                descInput.value = descEditable.innerText.trim();
            });
        }
    }

    _syncAllEditables() {
        this._initEditableSync();
    }

    _getFieldIndex(fieldElement) {
        return Array.from(this.fieldsContainer.children).indexOf(fieldElement);
    }

    _createOptionHtml(fieldIndex, optionIndex) {
        return `
            <div class="option-row row mb-2 align-items-center">
                <div class="col-md-10">
                    <input name="Fields[${fieldIndex}].Options[${optionIndex}]" 
                           class="form-control option-input" placeholder="Вариант ${optionIndex + 1}" />
                </div>
                <div class="col-md-2">
                    <button type="button" class="btn btn-sm btn-outline-danger remove-option">×</button>
                </div>
            </div>
        `;
    }

    _updateFieldIndex(name, newIndex) {
        // Fields[__NEW__123] -> Fields[newIndex]
        // Fields[0] -> Fields[newIndex]
        return name.replace(/Fields\[[^\]]+\]/, `Fields[${newIndex}]`);
    }

    _updateOptionIndex(name, fieldIndex, optIndex) {
        return name.replace(
            /Fields\[[^\]]+\]\.Options\[\d+\]/,
            `Fields[${fieldIndex}].Options[${optIndex}]`
        );
    }

    _triggerEvent(eventName, detail) {
        this.fieldsContainer?.dispatchEvent(new CustomEvent(eventName, { detail }));
    }

    _defaultOnSuccess(result) {
        if (this.resultDiv) {
            this.resultDiv.innerHTML = '<div class="alert alert-success">Тест сохранён!</div>';
        }
    }

    _defaultOnError(error) {
        console.error('TestEditManager Error:', error);
        if (this.resultDiv) {
            this.resultDiv.innerHTML = '<div class="alert alert-danger">' + 
                (error.message || 'Ошибка сохранения') + '</div>';
        }
    }
}
