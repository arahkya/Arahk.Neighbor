import React from 'react';
import Link from 'next/link';

interface House {
    id: string;
    addressName: string;
}

async function getHousesCollection(){
    const response = await fetch('http://localhost:5250/api/House', {
        cache: 'no-cache',
    });
    const houseList = await response.json();
    const houses: House[] = houseList as House[];

    return houses;
}

export default async function HouseList() {
    const houses: House[] = await getHousesCollection();
    
    return (
        <div className="grid grid-cols-12 gap-2 px-6">
            <div className="col-span-2 ml-2 mt-3 flex flex-col">
                <Link href="/house">House</Link>
            </div>
            <div className="bg-white col-span-10 rounded pb-4 flex flex-col">
                <div className="text-xl text-black pt-3 px-4">House List</div>

                <div className="my-2">
                    <Link href="/house/add" className="mx-4 py-1 text-sm rounded px-4 bg-gray-400">Add</Link>
                </div>

                <div className="grid grid-cols-12 gap-0 px-4 py-1 my-3 text-black bg-gray-100">
                    <div className="col-span-4 text-center text-gray-500">Id</div>
                    <div className="col-span-2 text-center text-gray-500">Address</div>
                    <div className="col-span-6"></div>
                </div>

                {houses.map((house: House, index: number) => (
                    <React.Fragment key={index}>
                        <div className="grid grid-cols-12 gap-0 px-4 py-1 text-black">
                            <div className="col-span-4 text-center py-1 text-gray-500">{house.id}</div>
                            <div className="col-span-2 text-center py-1 text-gray-500">{house.addressName}</div>
                            <div className="col-span-6 "></div>
                        </div>
                    </React.Fragment>
                ))}
            </div>
        </div>
    )
}