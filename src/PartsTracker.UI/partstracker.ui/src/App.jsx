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
        quantityOnHand: '',
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
                quantityOnHand: modalPart.quantityOnHand || '',
                locationCode: modalPart.locationCode || '',
                lastStockTake: new Date(modalPart.lastStockTake).toISOString().split('T')[0] || new Date().toISOString().split('T')[0]
            });
        } else if (showModal && modalMode === 'add') {
            setFormData({
                partNumber: '',
                description: '',
                quantityOnHand: '',
                locationCode: '',
                lastStockTake: new Date().toISOString().split('T')[0]
            });
        }
    }, [showModal, modalMode, modalPart]);

    const contents = loading
        ? <div class="d-flex justify-content-center align-items-center" style={{ minHeight: 200 }}>
            <div class="spinner-border text-primary"></div>
        </div>
        : <div class="table-responsive">
            <table class="table" aria-labelledby="tableLabel">
                <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">Description</th>
                        <th scope="col">Quantity</th>
                        <th scope="col">Location</th>
                        <th scope="col">Stock Take</th>
                        <th scope="col">
                        </th>
                    </tr>
                </thead>
                <tbody>
                    {parts.map(part =>
                        <tr key={part.partNumber}>
                            <td scope="row">{part.partNumber}</td>
                            <td>{part.description}</td>
                            <td>{part.quantityOnHand}</td>
                            <td>{part.locationCode}</td>
                            <td>{part.lastStockTake}</td>
                            <td>
                                <div class="btn-group btn-group-sm" role="group" aria-label="part_group">
                                    <button type="button" class="btn btn-primary" onClick={() => handleEditClick(part.partNumber)}><i class="bi bi-pen"></i></button>
                                    <button type="button" class="btn btn-danger" onClick={() => handleRemoveClick(part.partNumber)}><i class="bi bi-trash"></i></button>
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
        const part = parts.find(p => p.partNumber === partNumber);
        setModalMode('edit');
        setModalPart(part);
        setShowModal(true);
    }

    function handleModalClose() {
        setShowModal(false);
    }

    function handleFormChange(e) {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    }

    function handleFormClear() {
        setFormData({
            partNumber: '',
            description: '',
            quantityOnHand: '',
            locationCode: '',
        });
        setFormError('');
    }

    async function handleModalSave() {
        if (!formData.partNumber.trim()) {
            setFormError('Part Number is required.');
            return;
        }
        if (!formData.description.trim()) {
            setFormError('Description is required.');
            return;
        }
        if (formData.quantityOnHand === '' || formData.quantityOnHand === null) {
            setFormError('Quantity On Hand is required.');
            return;
        }
        setFormError('');
        let response;
        if (modalMode === 'add') {
            let now = new Date();
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
            let now = new Date();
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
        } else {
            setFormError('Failed to save changes.');
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
            {formError && <div class="alert alert-danger">{formError}</div>}
            <div class="form-group">
                <label>Part Number:{' '}
                    <input type="text" class="form-control" name="partNumber" value={formData.partNumber} onChange={handleFormChange} disabled={modalMode === 'edit'} />
                </label>
            </div>
            <div class="form-group">
                <label>Description:{' '}
                    <input type="text" class="form-control" name="description" value={formData.description} onChange={handleFormChange} />
                </label>
            </div>
            <div class="form-group">
                <label>Quantity On Hand:{' '}
                    <input type="number" class="form-control" name="quantityOnHand" value={formData.quantityOnHand} onChange={handleFormChange} />
                </label>
            </div>
            <div class="form-group">
                <label>Location:{' '}
                    <input type="text" class="form-control" name="locationCode" value={formData.locationCode} onChange={handleFormChange} />
                </label>
            </div>        
        </form>
    );

    return (
        <div class="container">
            <div class="row">
                <div class="col-12">
                    {contents}
                </div>
            </div>
            <div class="row">
                <div class="col-12">
                    <div class="position-fixed bottom-0 end-0 mb-3 me-3">
                        <button type="button" class="btn btn-primary" onClick={handleAddClick}>Add</button>
                    </div>
                </div>
            </div>
            {showModal && (
                <dialog class="modal show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">{modalTitle}</h5>
                                <button type="button" class="close" onClick={handleModalClose}>&times;</button>
                            </div>
                            <div class="modal-body">
                                {modalBody}
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" onClick={handleModalClose}>Close</button>
                                <button type="button" class="btn btn-warning" onClick={handleFormClear}>Clear</button>
                                <button type="button" class="btn btn-primary" onClick={handleModalSave} disabled={!isFormValid()}>Save changes</button>
                            </div>
                        </div>
                    </div>
                </dialog>
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
