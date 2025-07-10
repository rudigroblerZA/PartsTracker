import { useEffect, useState } from 'react';
import './App.css';

const server_url = 'http://localhost:8080';

function App() {
    const [parts, setParts] = useState();
    const [showModal, setShowModal] = useState(false);
    const [modalMode, setModalMode] = useState('add');
    const [modalPart, setModalPart] = useState(null);
    const [formData, setFormData] = useState({
        partNumber: '',
        description: '',
        quantityOnHand: 0,
        locationCode: '',
        lastStockTake: ''
    });
    const [formError, setFormError] = useState('');
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        populatePartsData();
    }, []);

    useEffect(() => {
        if (showModal && modalMode === 'edit' && modalPart) {
            setFormData({
                partNumber: modalPart.partNumber || '',
                description: modalPart.description || '',
                quantityOnHand: modalPart.quantityOnHand || 0,
                locationCode: modalPart.locationCode || '',
                lastStockTake: formatDate(modalPart.lastStockTake)
            });
        } else if (showModal && modalMode === 'add') {
            setFormData({
                partNumber: '',
                description: '',
                quantityOnHand: 0,
                locationCode: '',
                lastStockTake: formatDate(new Date().toISOString())
            });
        }
    }, [showModal, modalMode, modalPart]);

    const formatDate = (dateString) => {
        if (!dateString) return '';
        const date = new Date(dateString);

        let timeStr = date.toLocaleTimeString('en-ZA', {
            hour: '2-digit',
            minute: '2-digit',
            hour12: false,
            second: undefined
        });

        let dateStr = date.toLocaleDateString('en-ZA', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        });

        return dateStr + ' at ' + timeStr;

    }

    const contents = loading
        ? <div className="d-flex justify-content-center align-items-center" style={{ minHeight: 200 }}>
            <div className="spinner-border text-primary"></div>
        </div>
        : <div className="table-responsive">
            <table className="table" aria-labelledby="tableLabel">
                <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">Description</th>
                        <th scope="col">Quantity on hand</th>
                        <th scope="col">Location</th>
                        <th scope="col">Last Stock Take</th>
                        <th scope="col">
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {parts?.map(part =>
                        <tr key={part.partNumber}>
                            <td scope="row">{part.partNumber}</td>
                            <td>{part.description}</td>
                            <td>{part.quantityOnHand}</td>
                            <td>{part.locationCode}</td>
                            <td>{formatDate(part.lastStockTake)}</td>
                            <td>
                                <div className="btn-group btn-group-sm" role="group" aria-label="part_options_group">
                                    <button type="button" className="btn btn-primary" onClick={() => handleEditClick(part.partNumber)}><i className="bi bi-pen"></i></button>
                                    <button type="button" className="btn btn-danger" onClick={() => handleRemoveClick(part.partNumber)}><i className="bi bi-trash"></i></button>
                                </div>
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>;


    function handleAddClick() {
        setModalMode('add');
        setModalPart(null);
        setShowModal(true);
    }

    async function handleRemoveClick(partNumber) {
        const response = await fetch(`${server_url}/api/parts/${partNumber}`, {
            method: 'DELETE'
        });
        if (response.ok) {
            await populatePartsData();
        } else {
            alert('Failed to delete part.');
        }
    }

    function handleEditClick(partNumber) {
        if (parts) {
            const part = parts.find(p => p.partNumber === partNumber);
            setModalMode('edit');
            setModalPart(part);
            setShowModal(true);           
        }
    }

    function handleModalClose() {
        setShowModal(false);
    }

    function handleFormChange(e) {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    }

    async function handleModalSave() {
        const errors = [];
        if (!formData.partNumber.trim()) {
            errors.push('Part Number is required.');
        }
        if (!formData.description.trim()) {
            errors.push('Description is required.');
        }
        if (formData.quantityOnHand === '' || formData.quantityOnHand === null) {
            errors.push('Quantity On Hand is required.');
        } else if (isNaN(Number(formData.quantityOnHand))) {
            errors.push('Quantity On Hand must be a number.');
        }
        if (errors.length > 0) {
            setFormError(
                <ul className="mb-0">
                    {errors.map((err, i) => <li key={i}>{err}</li>)}
                </ul>
            );
            return;
        }
        setFormError('');
        let response;
        let now = new Date();
        if (modalMode === 'add') {
            response = await fetch(`${server_url}/api/parts`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    ...formData,
                    lastStockTake: now.toISOString(),
                    quantityOnHand: Number(formData.quantityOnHand)
                })
            });
        } else if (modalMode === 'edit') {
            response = await fetch(`${server_url}/api/parts/${formData.partNumber}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    ...formData,
                    lastStockTake: now.toISOString(),
                    quantityOnHand: Number(formData.quantityOnHand)
                })
            });
        }
        if (response?.ok) {
            setShowModal(false);
            await populatePartsData();
        } else if (response?.status === 409) {
            // Conflict (e.g. part already exists)
            const data = await response.json().catch(() => ({}));
            setFormError(data?.description || 'Part already exists.');
        } else {
            const data = await response.json().catch(() => ({}));
            if (data && data.errors) {
                Object.keys(data.errors).forEach(key => {
                    setFormError(prev => `${prev} - ${data.errors[key]}`);
                });
            } else {
                setFormError('Failed to save changes.');
            }
        }
    }

    function isFormValid() {
        return (
            formData.partNumber.trim() !== '' &&
            formData.description.trim() !== '' &&
            formData.quantityOnHand !== '' &&
            formData.quantityOnHand !== null
        );
    }

    const modalTitle = modalMode === 'add' ? 'Add Part' : 'Edit Part';

    const modalBody = (
        <form>
            {formError && <div className="alert alert-danger">{formError}</div>}

            <div className="form-group">
                <label htmlFor="partNumber">Part Number:</label>
                <input
                    type="text"
                    className={`form-control${formData.partNumber.trim() === '' ? ' is-invalid' : ''}`}
                    name="partNumber"
                    value={formData.partNumber}
                    onChange={handleFormChange}
                    disabled={modalMode === 'edit'}
                    required
                />
                {formData.partNumber.trim() === '' && (
                    <div className="invalid-feedback">Part Number is required.</div>
                )}
            </div>
            <div className="form-group">
                <label htmlFor="description">Description:</label>
                <input
                    type="text"
                    className={`form-control${formData.description.trim() === '' ? ' is-invalid' : ''}`}
                    name="description"
                    value={formData.description}
                    onChange={handleFormChange}
                    required
                />
                {formData.description.trim() === '' && (
                    <div className="invalid-feedback">Description is required.</div>
                )}
            </div>
            <div className="form-group">
                <label htmlFor="quantityOnHand">Quantity On Hand:</label>
                <input
                    type="number"
                    className={`form-control${formData.quantityOnHand === '' || isNaN(Number(formData.quantityOnHand)) ? ' is-invalid' : ''}`}
                    name="quantityOnHand"
                    value={formData.quantityOnHand}
                    onChange={handleFormChange}
                    min="0"
                    required
                />
                {(formData.quantityOnHand === '' || isNaN(Number(formData.quantityOnHand))) && (
                    <div className="invalid-feedback">Quantity On Hand is required and must be a number.</div>
                )}
            </div>
            <div className="form-group">
                <label htmlFor="locationCode">Location:</label>
                <input
                    type="text"
                    className="form-control"
                    name="locationCode"
                    value={formData.locationCode}
                    onChange={handleFormChange}
                />
            </div>
            <div className="form-group">
                <label htmlFor="lastStockTake">Last Stock Take:</label>
                <input
                    type="text"
                    className="form-control"
                    name="locationCode"
                    value={formData.lastStockTake}
                    disabled
                />
            </div>
        </form>
    );


    return (
        <div className="container">
            <div className="row">
                <div className="col-12">
                    {contents}
                </div>
            </div>
            <div className="row">
                <div className="col-12">
                    <div className="position-fixed bottom-0 end-0 mb-3 me-3">
                        <button type="button" className="btn btn-primary" onClick={handleAddClick}><i class="bi bi-file-earmark-plus"></i></button>
                    </div>
                </div>
            </div>
            {showModal && (
                <div className="modal show d-block" tabIndex="-1">
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">{modalTitle}</h5>
                                <button type="button" className="btn-close" onClick={handleModalClose} aria-label="Close"></button>
                            </div>
                            <div className="modal-body">
                                {modalBody}
                            </div>
                            <div className="modal-footer">
                                <button type="button" className="btn btn-secondary" onClick={handleModalClose}>Close</button>
                                <button type="button" className="btn btn-primary" onClick={handleModalSave} disabled={!isFormValid()}>Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );

    async function populatePartsData() {
        setLoading(true);
        const response = await fetch(`${server_url}/api/parts`);
        if (response.ok) {
            const data = await response.json();
            setParts(data);
        }
        setLoading(false);
    }
}

export default App
