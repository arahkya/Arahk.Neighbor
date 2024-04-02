'use client';
import Link from "next/link";

export default function AddHouse() {
    return (
        <div className="grid grid-cols-12 gap-2 px-6">
            <div className="col-span-2 ml-2 mt-3 flex flex-col">
                <Link href="/house">House</Link>
            </div>
            <div className="bg-white col-span-10 rounded pb-4 flex flex-col">
                <div className="text-xl text-black pt-3 px-4">Add House</div>

                <div className="flex flex-col text-black items-center">
                    <div>
                        <label className="block text-gray-500">Address Name</label>
                        <input type="text" id="addressName" className="border rounded"></input>
                    </div>
                    <div className="my-3">
                        <button type="button" className="mt-3 bg-blue-300 px-5 h-7 rounded" onClick={CallAddHouse}>Add</button>
                        <Link href="/house" className="ml-3 px-5 rounded">Cancel</Link>
                    </div>
                </div>
            </div>
        </div>
    )
}

export async function CallAddHouse() {
    const addressNameEle = document.getElementById('addressName')!
    const addressName = (addressNameEle as HTMLInputElement).value;

    const response = await fetch('http://localhost:5250/api/House', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ addressName })
    });

    if (response.ok) {
        console.log('House added successfully');
    } else {
        console.error('Error adding house');
    }
}