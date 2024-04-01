'use client'

import React, { useEffect, useState } from 'react';

export default function HouseList() {

    const houseList = FetchHouseList();

    return (
        <div className="grid grid-cols-12 gap-2">
            <div className="col-span-2 ml-3 mt-3 flex flex-col">
                <a href="/house">House</a>
                <a href="/house/add" className="ml-3">Add</a>
            </div>
            <div className="bg-white col-span-10 rounded">
                <div className="text-xl text-black p-4">House List</div>

                <div className="grid grid-cols-12 gap-2 p-4 text-black">
                    <div className="col-span-1">Id</div>
                    <div className="col-span-2">Address</div>
                </div>

                {houseList.map((house, index) => (
                    <React.Fragment key={index}>
                        <div className="text-black">
                            <div className="col-span-1">{house.id}</div>
                            <div className="col-span-2">{house.addressName}</div>
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

    console.log(houseList);
    return houseList;
}