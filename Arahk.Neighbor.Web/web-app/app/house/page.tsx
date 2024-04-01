'use client'

import React, { useEffect, useState } from 'react';

export default function HouseList() {

    const houseList = FetchHouseList();

    return (
        <div className="grid grid-cols-12 gap-2 px-6">
            <div className="col-span-2 ml-8 mt-3 flex flex-col">
                <a href="/house">House</a>
            </div>
            <div className="bg-white col-span-10 rounded pb-4 flex flex-col">
                <div className="text-xl text-black pt-3 px-4">House List</div>

                <div className="my-2">
                    <a href="/house/add" className="mx-4 py-1 text-sm rounded px-4 bg-gray-400">Add</a>
                </div>

                <div className="grid grid-cols-12 gap-0 px-4 py-1 my-3 text-black bg-gray-100">
                    <div className="col-span-4 text-center text-gray-500">Id</div>
                    <div className="col-span-2 text-center text-gray-500">Address</div>
                    <div className="col-span-6"></div>
                </div>

                {houseList.map((house: any, index: number) => (
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

function FetchHouseList() {
    const [houseList, setHouseList] = useState([]);
    useEffect(() => {
        fetch('http://localhost:5250/api/House')
            .then(response => response.json())
            .then(data => setHouseList(data));
    }, []);
    
    return houseList;
}